using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Passives
{
    /// <summary>
    /// Pasiva del Guardallama / Guardatumbas: parte del daño que hace le cura,
    /// más cuanta menos vida le queda (de <see cref="minHealFraction"/> con la
    /// vida llena a <see cref="maxHealFraction"/> al borde de la muerte).
    /// </summary>
    [RequireComponent(typeof(StatusEffects), typeof(Health))]
    public sealed class ZealPassive : MonoBehaviour, IDamageModifier
    {
        public float minHealFraction = 0.1f;
        public float maxHealFraction = 0.4f;

        private StatusEffects status;
        private Health health;

        private void Awake()
        {
            status = GetComponent<StatusEffects>();
            health = GetComponent<Health>();
        }

        private void OnEnable() => status.AddModifier(this);
        private void OnDisable() => status.RemoveModifier(this);

        public float ModifyOutgoing(float amount, Health victim, string source)
        {
            float missing = 1f - Mathf.Clamp01(health.Fraction);
            health.Heal(amount * Mathf.Lerp(minHealFraction, maxHealFraction, missing));
            return amount;
        }

        public float ModifyIncoming(float amount, Health attacker, Vector3 hitDirection, Health self) => amount;
    }
}
