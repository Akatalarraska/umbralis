using System;
using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Recurso de clase (Rabia, Maná, Concentración...). Un solo componente con
    /// los parámetros suficientes para describir cualquiera de ellos; la Rabia
    /// del Conquistador es: empieza a 0, sube al golpear y al recibir daño, y
    /// baja poco a poco fuera de combate. Las habilidades lo gastan o lo generan
    /// a través de <see cref="AbilityCaster"/>.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public sealed class ClassResource : MonoBehaviour
    {
        [Header("Recurso")]
        [SerializeField] private string displayName = "Rabia";
        [SerializeField, Min(1f)] private float max = 100f;
        [Tooltip("Valor al empezar (0 para Rabia, el máximo para Maná).")]
        [SerializeField, Min(0f)] private float startValue = 0f;

        [Header("Generación")]
        [Tooltip("Puntos ganados por cada golpe que el dueño acierta, sea la habilidad que sea.")]
        [SerializeField, Min(0f)] private float gainPerHitDealt = 5f;
        [Tooltip("Puntos ganados por cada punto de daño recibido.")]
        [SerializeField, Min(0f)] private float gainPerDamageTaken = 0.1f;

        [Header("Fuera de combate")]
        [Tooltip("Segundos sin dar ni recibir daño para considerarse fuera de combate.")]
        [SerializeField, Min(0f)] private float combatTimeout = 5f;
        [Tooltip("Puntos por segundo que baja fuera de combate (negativo para que suba, como el Maná).")]
        [SerializeField] private float outOfCombatChangePerSecond = -8f;

        public string DisplayName => displayName;
        public float Max => max;
        public float Current { get; private set; }
        public float Fraction => Current / max;
        public bool InCombat => Time.time - lastCombatTime < combatTimeout;

        /// <summary>Se dispara cada vez que cambia el valor.</summary>
        public event Action Changed;

        private Health health;
        private float lastCombatTime = float.NegativeInfinity;

        private void Awake()
        {
            health = GetComponent<Health>();
            Current = Mathf.Clamp(startValue, 0f, max);
        }

        private void OnEnable()
        {
            health.Damaged += OnDamageTaken;
            Health.AnyDamaged += OnAnyDamaged;
        }

        private void OnDisable()
        {
            health.Damaged -= OnDamageTaken;
            Health.AnyDamaged -= OnAnyDamaged;
        }

        private void Update()
        {
            if (InCombat || outOfCombatChangePerSecond == 0f) return;
            Set(Current + outOfCombatChangePerSecond * Time.deltaTime);
        }

        public bool CanSpend(float amount) => amount <= 0f || Current >= amount;

        /// <summary>Gasta el recurso si hay suficiente. Devuelve false y no toca nada si no lo hay.</summary>
        public bool TrySpend(float amount)
        {
            if (!CanSpend(amount)) return false;
            if (amount > 0f)
            {
                Set(Current - amount);
                MarkCombat();
            }
            return true;
        }

        public void Gain(float amount)
        {
            if (amount <= 0f) return;
            Set(Current + amount);
            MarkCombat();
        }

        /// <summary>Gasta todo lo que hay y devuelve cuánto era (para el Golpe del Conquistador).</summary>
        public float SpendAll()
        {
            float spent = Current;
            Set(0f);
            MarkCombat();
            return spent;
        }

        private void OnDamageTaken(float amount, Vector3 _)
        {
            MarkCombat();
            Gain(amount * gainPerDamageTaken);
        }

        private void OnAnyDamaged(Health attacker, Health victim, float amount, string source)
        {
            if (attacker != health) return;
            MarkCombat();
            Gain(gainPerHitDealt);
        }

        private void MarkCombat() => lastCombatTime = Time.time;

        private void Set(float value)
        {
            value = Mathf.Clamp(value, 0f, max);
            if (Mathf.Approximately(value, Current)) return;
            Current = value;
            Changed?.Invoke();
        }
    }
}
