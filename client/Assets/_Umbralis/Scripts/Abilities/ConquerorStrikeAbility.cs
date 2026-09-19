using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Golpe del Conquistador (definitiva): necesita objetivo. El personaje se
    /// queda inmóvil cargando <see cref="chargeDuration"/> segundos, gasta toda
    /// la Rabia al empezar y al terminar descarga un golpe enorme que crece con
    /// la Rabia gastada. Si el objetivo se aleja del alcance, muere, o al
    /// jugador lo aturden o interrumpen, el golpe falla y la recarga corre
    /// igual. Es el pago del combo: aturdir primero para asegurarlo.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Definitivas/Golpe del Conquistador", fileName = "ConquerorStrike")]
    public sealed class ConquerorStrikeAbility : AbilityDefinition
    {
        [Header("Golpe del Conquistador")]
        [Min(0.1f)] public float chargeDuration = 1f;
        [Tooltip("Rabia mínima para poder empezar la carga.")]
        [Min(0f)] public float minimumResource = 30f;
        [Tooltip("Daño extra por cada punto de Rabia gastado (se suma al daño base).")]
        [Min(0f)] public float damagePerResource = 2f;

        public override bool CanExecute(in AbilityContext context)
        {
            if (context.Target == null || !context.Target.IsAlive) return false;
            ClassResource resource = context.Caster.Resource;
            return resource == null || resource.Current >= minimumResource;
        }

        public override void Execute(in AbilityContext context)
        {
            AbilityCaster caster = context.Caster;
            Health target = context.Target;
            float spent = caster.Resource != null ? caster.Resource.SpendAll() : 0f;
            float finalDamage = damage + spent * damagePerResource;

            // Carga: una barra fina entre el personaje y el objetivo mientras dura.
            Vector3 to = target.transform.position - caster.transform.position;
            to.y = 0f;
            SpawnFx(PrimitiveType.Cube, context.Origin + to * 0.5f, Quaternion.LookRotation(to.normalized, Vector3.up),
                new Vector3(0.15f, 0.15f, to.magnitude), chargeDuration);

            caster.StartChannel(chargeDuration,
                onComplete: () =>
                {
                    Vector3 now = target.transform.position - caster.transform.position;
                    now.y = 0f;
                    bool inReach = target.IsAlive && now.magnitude <= range * 1.25f;
                    if (!inReach) { Fail(caster); return; }

                    caster.Movement.LockFacing(now, 0.3f);
                    caster.Health.DealDamage(target, finalDamage, now.normalized, displayName);
                    SpawnFx(PrimitiveType.Sphere, target.transform.position + Vector3.up, Quaternion.identity, Vector3.one * 2.5f, 0.2f);
                },
                onInterrupt: () => Fail(caster));
        }

        private void Fail(AbilityCaster caster)
        {
            // Fallo: un cubo pequeño delante del personaje que se esfuma. Sin sonido todavía.
            SpawnFx(PrimitiveType.Cube, caster.Origin + caster.transform.forward, Quaternion.identity, Vector3.one * 0.6f, 0.3f);
        }
    }
}
