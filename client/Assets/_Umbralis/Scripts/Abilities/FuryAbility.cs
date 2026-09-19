using UnityEngine;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Furia (definitiva): durante unos segundos el personaje hace más daño y
    /// roba vida con cada golpe. No se apunta. La velocidad de ataque del
    /// diseño queda pendiente: no hay animaciones ni tiempo de golpe todavía.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Definitivas/Furia", fileName = "Fury")]
    public sealed class FuryAbility : AbilityDefinition
    {
        [Header("Furia")]
        [Min(0.5f)] public float duration = 8f;
        [Min(1f)] public float damageMultiplier = 1.3f;
        [Tooltip("Fracción del daño hecho que se recupera como vida.")]
        [Range(0f, 1f)] public float lifesteal = 0.25f;

        public override void Execute(in AbilityContext context)
        {
            if (context.Caster.Status != null) context.Caster.Status.ApplyBuff(damageMultiplier, lifesteal, duration);

            // Aura: una esfera grande que se ve un instante al activar.
            SpawnFx(PrimitiveType.Sphere, context.Origin, Quaternion.identity, Vector3.one * 3f, 0.25f);
        }
    }
}
