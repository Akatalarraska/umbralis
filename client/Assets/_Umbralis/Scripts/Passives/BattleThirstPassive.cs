using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Passives
{
    /// <summary>
    /// Sed de batalla (pasiva del Devastador): por encima de cierta Rabia los
    /// ataques básicos hacen más daño. Reconoce el básico por su nombre, que le
    /// pasa la especialización al activarse.
    /// </summary>
    [RequireComponent(typeof(StatusEffects))]
    public sealed class BattleThirstPassive : MonoBehaviour, IDamageModifier
    {
        public string basicAttackName;
        public float resourceThreshold = 50f;
        public float basicMultiplier = 1.25f;

        public bool IsActive => resource != null && resource.Current > resourceThreshold;

        private StatusEffects status;
        private ClassResource resource;

        private void Awake()
        {
            status = GetComponent<StatusEffects>();
            resource = GetComponent<ClassResource>();
        }

        private void OnEnable() => status.AddModifier(this);
        private void OnDisable() => status.RemoveModifier(this);

        public float ModifyOutgoing(float amount, Health victim, string source)
        {
            return IsActive && source == basicAttackName ? amount * basicMultiplier : amount;
        }

        public float ModifyIncoming(float amount, Health attacker, Vector3 hitDirection, Health self) => amount;
    }
}
