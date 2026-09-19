using UnityEngine;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Una especialización por datos: ataque básico, sus 10 habilidades, sus 2
    /// definitivas, la pasiva y qué 6 + 1 van en la barra por defecto. El
    /// jugador elegirá las suyas más adelante; de momento la barra es la de
    /// <see cref="defaultAbilityIndices"/> y <see cref="defaultUltimateIndex"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Especialización", fileName = "Especializacion")]
    public sealed class SpecializationDefinition : ScriptableObject
    {
        public string displayName = "Especialización";
        [TextArea] public string description;

        [Header("Kit")]
        public AbilityDefinition basicAttack;
        public AbilityDefinition[] abilities = new AbilityDefinition[10];
        public AbilityDefinition[] ultimates = new AbilityDefinition[2];
        public PassiveDefinition passive;

        [Header("Barra por defecto")]
        [Tooltip("Índices en 'abilities' de las 6 que van en la barra.")]
        public int[] defaultAbilityIndices = { 0, 1, 2, 3, 4, 5 };
        [Tooltip("Índice en 'ultimates' de la definitiva elegida.")]
        public int defaultUltimateIndex = 0;

        /// <summary>Construye las 8 ranuras de la barra (0 básico, 1..6 habilidades, 7 definitiva).</summary>
        public AbilityDefinition[] BuildSlots()
        {
            var slots = new AbilityDefinition[AbilityCaster.TotalSlots];
            slots[AbilityCaster.BasicSlot] = basicAttack;
            for (int i = 0; i < AbilityCaster.AbilitySlotCount; i++)
            {
                int index = i < defaultAbilityIndices.Length ? defaultAbilityIndices[i] : -1;
                slots[AbilityCaster.FirstAbilitySlot + i] = index >= 0 && index < abilities.Length ? abilities[index] : null;
            }
            slots[AbilityCaster.UltimateSlot] = defaultUltimateIndex >= 0 && defaultUltimateIndex < ultimates.Length ? ultimates[defaultUltimateIndex] : null;
            return slots;
        }
    }
}
