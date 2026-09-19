using System;
using System.Collections.Generic;
using UnityEngine;

namespace Umbralis.Combat
{
    public enum Team { Player, Enemy }

    /// <summary>
    /// Puntos de vida de cualquier cosa que pueda recibir daño (jugador, muñecos,
    /// futuros enemigos). Mantiene un registro estático de todos los vivos para
    /// que las habilidades busquen objetivos sin etiquetas ni capas.
    /// </summary>
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private Team team = Team.Enemy;
        [SerializeField, Min(1f)] private float maxHealth = 100f;

        public Team Team => team;
        public float Max => maxHealth;
        public float Current { get; private set; }
        public float Fraction => Current / maxHealth;
        public bool IsAlive => Current > 0f;

        /// <summary>Daño recibido y dirección horizontal desde la que vino (normalizada).</summary>
        public event Action<float, Vector3> Damaged;
        public event Action Died;
        public event Action Revived;

        /// <summary>Todos los Health activos en la escena.</summary>
        public static readonly List<Health> All = new List<Health>();

        private void Awake() => Current = maxHealth;
        private void OnEnable() => All.Add(this);
        private void OnDisable() => All.Remove(this);

        public void TakeDamage(float amount, Vector3 hitDirection)
        {
            if (!IsAlive || amount <= 0f) return;

            Current = Mathf.Max(0f, Current - amount);
            hitDirection.y = 0f;
            Damaged?.Invoke(amount, hitDirection.sqrMagnitude > 0.0001f ? hitDirection.normalized : Vector3.zero);

            if (!IsAlive) Died?.Invoke();
        }

        public void Revive()
        {
            Current = maxHealth;
            Revived?.Invoke();
        }

        /// <summary>
        /// Enemigo vivo más cercano a <paramref name="origin"/> dentro de
        /// <paramref name="range"/> que no sea del equipo indicado. Null si no hay.
        /// </summary>
        public static Health FindNearestHostile(Team myTeam, Vector3 origin, float range)
        {
            Health best = null;
            float bestSqr = range * range;
            foreach (Health h in All)
            {
                if (h.team == myTeam || !h.IsAlive) continue;
                float sqr = (h.transform.position - origin).sqrMagnitude;
                if (sqr < bestSqr) { bestSqr = sqr; best = h; }
            }
            return best;
        }
    }
}
