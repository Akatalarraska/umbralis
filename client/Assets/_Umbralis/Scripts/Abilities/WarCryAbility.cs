using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Grito de guerra: genera recurso (via <c>resourceGain</c>) y ralentiza a
    /// los enemigos cercanos. No se apunta: se lanza al instante alrededor del
    /// personaje. <c>range</c> es el radio del grito.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Grito de guerra", fileName = "WarCry")]
    public sealed class WarCryAbility : AbilityDefinition
    {
        [Header("Grito")]
        [Tooltip("Fracción de velocidad que pierden los enemigos (0,4 = 40 % más lentos).")]
        [Range(0f, 1f)] public float slowFraction = 0.4f;
        [Min(0f)] public float slowDuration = 4f;

        public override void Execute(in AbilityContext context)
        {
            Vector3 center = context.Caster.transform.position;
            Team team = context.Caster.Team;
            foreach (Health h in Health.All.ToArray())
            {
                if (h.Team == team || !h.IsAlive) continue;
                if ((h.transform.position - center).sqrMagnitude > range * range) continue;
                StatusEffects status = h.GetComponent<StatusEffects>();
                if (status != null) status.ApplySlow(slowFraction, slowDuration);
                if (damage > 0f) context.Caster.Health.DealDamage(h, damage, (h.transform.position - center).normalized, displayName);
            }

            // Onda: un anillo plano que se ve un instante alrededor del personaje.
            SpawnFx(PrimitiveType.Cylinder, center + Vector3.up * 0.05f, Quaternion.identity, new Vector3(range * 2f, 0.03f, range * 2f), 0.2f);
        }
    }
}
