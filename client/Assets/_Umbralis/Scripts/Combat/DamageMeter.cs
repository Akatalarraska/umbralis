using System;
using System.Collections.Generic;
using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Medidor de daño del dueño: daño total, daño por segundo y desglose por
    /// habilidad. Una "sesión" empieza con el primer golpe y se congela tras
    /// <see cref="idleTimeout"/> segundos sin pegar; el siguiente golpe empieza
    /// una nueva. Es el medidor del muñeco de entrenamiento del diseño (solo
    /// del propio jugador, nunca público).
    /// </summary>
    [RequireComponent(typeof(Health))]
    public sealed class DamageMeter : MonoBehaviour
    {
        [Tooltip("Segundos sin hacer daño para dar la sesión por terminada.")]
        [SerializeField, Min(1f)] private float idleTimeout = 5f;

        public float Total { get; private set; }
        /// <summary>Segundos desde el primer golpe hasta el último (o hasta ahora si sigue activa).</summary>
        public float Duration => Total <= 0f ? 0f : (IsActive ? Time.time : lastHitTime) - firstHitTime;
        public float DamagePerSecond => Duration > 0.5f ? Total / Duration : Total;
        public bool IsActive => Total > 0f && Time.time - lastHitTime < idleTimeout;

        /// <summary>Daño acumulado por origen, de mayor a menor.</summary>
        public IReadOnlyList<KeyValuePair<string, float>> Breakdown
        {
            get
            {
                if (breakdownDirty)
                {
                    sorted.Clear();
                    sorted.AddRange(bySource);
                    sorted.Sort((a, b) => b.Value.CompareTo(a.Value));
                    breakdownDirty = false;
                }
                return sorted;
            }
        }

        public event Action Changed;

        private Health owner;
        private float firstHitTime, lastHitTime;
        private readonly Dictionary<string, float> bySource = new Dictionary<string, float>();
        private readonly List<KeyValuePair<string, float>> sorted = new List<KeyValuePair<string, float>>();
        private bool breakdownDirty;

        private void Awake() => owner = GetComponent<Health>();
        private void OnEnable() => Health.AnyDamaged += OnAnyDamaged;
        private void OnDisable() => Health.AnyDamaged -= OnAnyDamaged;

        public void Reset()
        {
            Total = 0f;
            bySource.Clear();
            breakdownDirty = true;
            Changed?.Invoke();
        }

        private void OnAnyDamaged(Health attacker, Health victim, float amount, string source)
        {
            if (attacker != owner || amount <= 0f) return;

            if (!IsActive) // nueva sesión
            {
                Total = 0f;
                bySource.Clear();
                firstHitTime = Time.time;
            }
            lastHitTime = Time.time;
            Total += amount;

            string key = string.IsNullOrEmpty(source) ? "Otros" : source;
            bySource.TryGetValue(key, out float acc);
            bySource[key] = acc + amount;
            breakdownDirty = true;
            Changed?.Invoke();
        }
    }
}
