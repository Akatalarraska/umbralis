using UnityEngine;
using Umbralis.Abilities;

namespace Umbralis.Passives
{
    /// <summary>Asset de la pasiva Muralla: crea <see cref="BulwarkPassive"/> en el personaje.</summary>
    [CreateAssetMenu(menuName = "Umbralis/Pasivas/Muralla", fileName = "Muralla")]
    public sealed class BulwarkPassiveDefinition : PassiveDefinition
    {
        [Range(0f, 1f)] public float frontalReduction = 0.3f;
        [Min(1)] public int maxCharges = 5;
        [Min(0f)] public float bonusPerCharge = 0.1f;

        public override Behaviour Attach(GameObject owner, SpecializationDefinition spec)
        {
            var passive = owner.AddComponent<BulwarkPassive>();
            passive.frontalReduction = frontalReduction;
            passive.maxCharges = maxCharges;
            passive.bonusPerCharge = bonusPerCharge;
            return passive;
        }
    }
}
