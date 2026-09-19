using UnityEngine;
using Umbralis.Abilities;

namespace Umbralis.Passives
{
    /// <summary>Asset de la pasiva Sed de batalla: crea <see cref="BattleThirstPassive"/> en el personaje.</summary>
    [CreateAssetMenu(menuName = "Umbralis/Pasivas/Sed de batalla", fileName = "SedDeBatalla")]
    public sealed class BattleThirstPassiveDefinition : PassiveDefinition
    {
        [Min(0f)] public float resourceThreshold = 50f;
        [Min(1f)] public float basicMultiplier = 1.25f;

        public override Behaviour Attach(GameObject owner, SpecializationDefinition spec)
        {
            var passive = owner.AddComponent<BattleThirstPassive>();
            passive.basicAttackName = spec != null && spec.basicAttack != null ? spec.basicAttack.displayName : string.Empty;
            passive.resourceThreshold = resourceThreshold;
            passive.basicMultiplier = basicMultiplier;
            return passive;
        }
    }
}
