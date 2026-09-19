using System;
using UnityEngine;
using Umbralis.Abilities;
using Umbralis.Combat;

namespace Umbralis.Passives
{
    /// <summary>
    /// Muralla (pasiva del Baluarte): recibe menos daño de frente, y cada golpe
    /// bloqueado así da una carga (hasta 5) que aumenta el daño del siguiente
    /// golpe que dé. Aplastar consume las cargas con un extra propio.
    /// </summary>
    [RequireComponent(typeof(StatusEffects))]
    public sealed class BulwarkPassive : MonoBehaviour, IDamageModifier
    {
        public float frontalReduction = 0.3f;
        public int maxCharges = 5;
        [Tooltip("Daño extra del siguiente golpe por cada carga (0,1 = +10 % por carga).")]
        public float bonusPerCharge = 0.1f;

        public int Charges { get; private set; }
        public event Action ChargesChanged;

        private StatusEffects status;

        private void Awake() => status = GetComponent<StatusEffects>();
        private void OnEnable() => status.AddModifier(this);
        private void OnDisable() => status.RemoveModifier(this);

        /// <summary>Se lleva todas las cargas (Aplastar). Devuelve cuántas había.</summary>
        public int ConsumeCharges()
        {
            int c = Charges;
            SetCharges(0);
            return c;
        }

        public float ModifyIncoming(float amount, Health attacker, Vector3 hitDirection, Health self)
        {
            if (!IsFrontal(attacker, hitDirection, self.transform)) return amount;
            SetCharges(Mathf.Min(Charges + 1, maxCharges));
            return amount * (1f - frontalReduction);
        }

        public float ModifyOutgoing(float amount, Health victim, string source)
        {
            if (Charges <= 0) return amount;
            float bonus = 1f + bonusPerCharge * Charges;
            SetCharges(0);
            return amount * bonus;
        }

        /// <summary>De frente = el atacante está delante; sin atacante, el golpe viene de delante.</summary>
        public static bool IsFrontal(Health attacker, Vector3 hitDirection, Transform self)
        {
            Vector3 from;
            if (attacker != null) from = attacker.transform.position - self.position;
            else from = -hitDirection;
            from.y = 0f;
            if (from.sqrMagnitude < 0.0001f) return true;
            return Vector3.Dot(self.forward, from.normalized) > 0f;
        }

        private void SetCharges(int value)
        {
            if (value == Charges) return;
            Charges = value;
            ChargesChanged?.Invoke();
        }
    }
}
