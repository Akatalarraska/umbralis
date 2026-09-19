using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Abilities
{
    /// <summary>Dispara una esfera en línea recta que daña al primer enemigo que toca.</summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Proyectil", fileName = "Projectile")]
    public sealed class ProjectileAbility : AbilityDefinition
    {
        [Header("Proyectil")]
        [Min(1f)] public float speed = 18f;
        [Min(0.05f)] public float radius = 0.25f;

        public override void Execute(in AbilityContext context)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = $"{displayName} Projectile";
            Object.Destroy(go.GetComponent<Collider>()); // el proyectil se detecta a sí mismo por SphereCast
            go.transform.position = context.Origin + context.Direction * 0.8f;
            go.transform.localScale = Vector3.one * (radius * 2f);
            if (fxMaterial != null) go.GetComponent<Renderer>().sharedMaterial = fxMaterial;

            go.AddComponent<Projectile>().Launch(
                context.Caster.Health, context.Direction, speed, range, damage, radius, displayName);
        }
    }
}
