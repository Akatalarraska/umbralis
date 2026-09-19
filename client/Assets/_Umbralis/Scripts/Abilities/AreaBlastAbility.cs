using UnityEngine;

namespace Umbralis.Abilities
{
    /// <summary>Estallido en un punto del suelo elegido dentro del alcance: daño en área y empujón hacia fuera.</summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Estallido (área)", fileName = "AreaBlast")]
    public sealed class AreaBlastAbility : AbilityDefinition
    {
        [Header("Estallido")]
        [Min(0.25f)] public float radius = 2.5f;

        public override float AimRadius => radius;

        private void OnValidate() => aimMode = AimMode.Point;

        public override void Execute(in AbilityContext context)
        {
            Vector3 point = context.Point;
            point.y = context.Caster.transform.position.y;

            // knockbackDirection = zero → cada objetivo sale despedido desde el centro.
            DamageInSphere(point, radius, damage, context.Caster.Health, Vector3.zero);

            SpawnFx(PrimitiveType.Sphere, point + Vector3.up * 0.5f, Quaternion.identity, Vector3.one * (radius * 2f), 0.15f);
        }
    }
}
