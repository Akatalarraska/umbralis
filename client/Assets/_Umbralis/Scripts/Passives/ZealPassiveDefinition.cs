using UnityEngine;
using Umbralis.Abilities;

namespace Umbralis.Passives
{
    /// <summary>Asset de la pasiva del Guardallama / Guardatumbas: crea <see cref="ZealPassive"/>.</summary>
    [CreateAssetMenu(menuName = "Umbralis/Pasivas/Fervor curativo", fileName = "FervorCurativo")]
    public sealed class ZealPassiveDefinition : PassiveDefinition
    {
        [Range(0f, 1f)] public float minHealFraction = 0.1f;
        [Range(0f, 1f)] public float maxHealFraction = 0.4f;

        public override Behaviour Attach(GameObject owner, SpecializationDefinition spec)
        {
            var passive = owner.AddComponent<ZealPassive>();
            passive.minHealFraction = minHealFraction;
            passive.maxHealFraction = maxHealFraction;
            return passive;
        }
    }
}
