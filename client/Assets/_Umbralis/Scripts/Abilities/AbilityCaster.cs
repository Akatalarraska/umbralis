using UnityEngine;
using Umbralis.Combat;
using Umbralis.Player;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Ranuras de habilidades del personaje y sus enfriamientos. Los botones del
    /// HUD le piden lanzar una ranura con una dirección/punto ya decididos (o sin
    /// nada, y entonces auto-apunta al enemigo más cercano).
    /// </summary>
    [RequireComponent(typeof(PlayerMovement), typeof(Health))]
    public sealed class AbilityCaster : MonoBehaviour
    {
        [SerializeField] private AbilityDefinition[] slots = new AbilityDefinition[4];

        [Tooltip("Altura desde los pies a la que nacen proyectiles y efectos.")]
        [SerializeField] private float originHeight = 1.1f;

        public PlayerMovement Movement { get; private set; }
        public Team Team => health.Team;
        public int SlotCount => slots.Length;
        public Vector3 Origin => transform.position + Vector3.up * originHeight;

        private Health health;
        private float[] cooldownEnds;

        private void Awake()
        {
            Movement = GetComponent<PlayerMovement>();
            health = GetComponent<Health>();
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

        public bool IsReady(int slot)
        {
            return GetAbility(slot) != null && Time.time >= cooldownEnds[slot];
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
            if (!IsReady(slot)) return false;
            AbilityDefinition ability = slots[slot];

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) direction = transform.forward;
            direction.Normalize();

            cooldownEnds[slot] = Time.time + ability.cooldown;
            Movement.LockFacing(direction, ability.faceLockDuration);

            ability.Execute(new AbilityContext(this, Origin, direction, point));
            return true;
        }
    }
}
