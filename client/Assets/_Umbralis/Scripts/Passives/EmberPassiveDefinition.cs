using UnityEngine;
using Umbralis.Abilities;
using Umbralis.Core;

namespace Umbralis.Passives
{
    /// <summary>Asset de Brasas del juicio / Almas errantes: crea <see cref="EmberPassive"/>.</summary>
    [CreateAssetMenu(menuName = "Umbralis/Pasivas/Brasas del juicio", fileName = "BrasasDelJuicio")]
    public sealed class EmberPassiveDefinition : PassiveDefinition
    {
        [Range(0f, 1f)] public float chance = 0.3f;
        [Range(0f, 1f)] public float healFraction = 0.05f;
        [Min(0f)] public float resourceGain = 10f;
        [Min(1f)] public float lifetime = 10f;
        [Tooltip("Brasas (Llama Blanca).")]
        public Material material;
        [Tooltip("Almas (Pacto Oscuro).")]
        public Material mirrorMaterial;

        public override Behaviour Attach(GameObject owner, SpecializationDefinition spec)
        {
            var passive = owner.AddComponent<EmberPassive>();
            passive.chance = chance;
            passive.healFraction = healFraction;
            passive.resourceGain = resourceGain;
            passive.lifetime = lifetime;
            passive.material = FactionSettings.IsPacto && mirrorMaterial != null ? mirrorMaterial : material;
            return passive;
        }
    }
}
