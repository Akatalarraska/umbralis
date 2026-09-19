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

        /// <summary>
        /// Cualquier daño de la escena: (atacante, víctima, cantidad). El atacante
        /// puede ser null (daño ambiental). Lo usan los recursos de clase, los
        /// números flotantes y, más adelante, el medidor de daño.
        /// </summary>
        public static event Action<Health, Health, float> AnyDamaged;

        /// <summary>Todos los Health activos en la escena.</summary>
        public static readonly List<Health> All = new List<Health>();

        private void Awake() => Current = maxHealth;
        private void OnEnable() => All.Add(this);
        private void OnDisable() => All.Remove(this);

        /// <param name="attacker">Quién hace el daño; null si no hay nadie (ambiental).</param>
        public void TakeDamage(float amount, Vector3 hitDirection, Health attacker = null)
        {
            if (!IsAlive || amount <= 0f) return;

            Current = Mathf.Max(0f, Current - amount);
            hitDirection.y = 0f;
            Damaged?.Invoke(amount, hitDirection.sqrMagnitude > 0.0001f ? hitDirection.normalized : Vector3.zero);
            AnyDamaged?.Invoke(attacker, this, amount);

            if (!IsAlive) Died?.Invoke();
        }

        public void Revive()
        {
            Current = maxHealth;
            Revived?.Invoke();
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f) return;
            Current = Mathf.Min(maxHealth, Current + amount);
        }

        /// <summary>
        /// Hace daño a <paramref name="victim"/> en nombre de este personaje,
        /// aplicando sus potenciaciones (multiplicador de daño, robo de vida).
        /// Todo el daño de habilidades pasa por aquí. Devuelve el daño aplicado.
        /// </summary>
        public float DealDamage(Health victim, float amount, Vector3 hitDirection)
        {
            if (victim == null || !victim.IsAlive || amount <= 0f) return 0f;

            if (statusEffects == null) statusEffects = GetComponent<StatusEffects>();
            float multiplier = statusEffects != null ? statusEffects.DamageMultiplier : 1f;
            float lifesteal = statusEffects != null ? statusEffects.Lifesteal : 0f;

            float dealt = Mathf.Min(amount * multiplier, victim.Current);
            victim.TakeDamage(amount * multiplier, hitDirection, this);
            if (lifesteal > 0f) Heal(dealt * lifesteal);
            return dealt;
        }

        private StatusEffects statusEffects;

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
