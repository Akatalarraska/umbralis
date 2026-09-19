using UnityEngine;
using Umbralis.Combat;
using Umbralis.Player;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Ranuras de habilidades del personaje y sus enfriamientos. Los botones del
    /// HUD le piden lanzar una ranura con una dirección/punto ya decididos (o sin
    /// nada, y entonces auto-apunta al enemigo más cercano). Si el personaje
    /// tiene <see cref="ClassResource"/>, cada lanzamiento paga su coste y
    /// genera lo que diga la habilidad.
    ///
    /// Orden de ranuras: 0 = ataque básico, 1..6 = habilidades, 7 = definitiva.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement), typeof(Health))]
    public sealed class AbilityCaster : MonoBehaviour
    {
        public const int BasicSlot = 0;
        public const int FirstAbilitySlot = 1;
        public const int AbilitySlotCount = 6;
        public const int UltimateSlot = 7;
        public const int TotalSlots = 8;

        [SerializeField] private AbilityDefinition[] slots = new AbilityDefinition[TotalSlots];

        [Tooltip("Altura desde los pies a la que nacen proyectiles y efectos.")]
        [SerializeField] private float originHeight = 1.1f;

        public PlayerMovement Movement { get; private set; }
        public Health Health { get; private set; }
        /// <summary>Recurso de clase; null si el personaje no tiene (entonces todo es gratis).</summary>
        public ClassResource Resource { get; private set; }
        public Team Team => Health.Team;
        public int SlotCount => slots.Length;
        public Vector3 Origin => transform.position + Vector3.up * originHeight;

        private float[] cooldownEnds;

        private void Awake()
        {
            Movement = GetComponent<PlayerMovement>();
            Health = GetComponent<Health>();
            Resource = GetComponent<ClassResource>();
            cooldownEnds = new float[slots.Length];
        }

        public AbilityDefinition GetAbility(int slot)
        {
            return slot >= 0 && slot < slots.Length ? slots[slot] : null;
        }

        /// <summary>0 = lista; 1 = acaba de lanzarse. Para el relleno del botón.</summary>
        public float CooldownFraction(int slot)
        {
            AbilityDefinition ability = GetAbility(slot);
            if (ability == null || ability.cooldown <= 0f) return 0f;
            float remaining = cooldownEnds[slot] - Time.time;
            return remaining <= 0f ? 0f : Mathf.Clamp01(remaining / ability.cooldown);
        }

        /// <summary>Fuera de enfriamiento (no mira el recurso).</summary>
        public bool IsReady(int slot)
        {
            return GetAbility(slot) != null && Time.time >= cooldownEnds[slot];
        }

        /// <summary>Hay recurso suficiente para pagarla.</summary>
        public bool CanAfford(int slot)
        {
            AbilityDefinition ability = GetAbility(slot);
            return ability != null && (Resource == null || Resource.CanSpend(ability.resourceCost));
        }

        /// <summary>Lanzamiento rápido (toque corto): apunta solo al enemigo más cercano.</summary>
        public bool TryCastAuto(int slot)
        {
            AbilityDefinition ability = GetAbility(slot);
            if (ability == null) return false;

            Vector3 direction = transform.forward;
            Vector3 point = transform.position + direction * ability.range;

            Health target = Health.FindNearestHostile(Team, transform.position, ability.range * 1.25f);
            if (target != null)
            {
                Vector3 to = target.transform.position - transform.position;
                to.y = 0f;
                if (to.sqrMagnitude > 0.0001f) direction = to.normalized;
                point = transform.position + Vector3.ClampMagnitude(to, ability.range);
            }

            return TryCast(slot, direction, point);
        }

        /// <summary>Lanza con una dirección (horizontal) y un punto ya elegidos por el jugador.</summary>
        public bool TryCast(int slot, Vector3 direction, Vector3 point)
        {
            if (!IsReady(slot) || !CanAfford(slot)) return false;
            AbilityDefinition ability = slots[slot];

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) direction = transform.forward;
            direction.Normalize();

            if (Resource != null)
            {
                Resource.TrySpend(ability.resourceCost);
                Resource.Gain(ability.resourceGain);
            }
            cooldownEnds[slot] = Time.time + ability.cooldown;
            Movement.LockFacing(direction, ability.faceLockDuration);

            ability.Execute(new AbilityContext(this, Origin, direction, point));
            return true;
        }
    }
}
