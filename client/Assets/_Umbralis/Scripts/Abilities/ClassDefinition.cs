using UnityEngine;
using Umbralis.Core;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Una clase por datos: nombre (y su espejo en el Pacto Oscuro), recurso,
    /// sus dos especializaciones y el color del cuerpo. Los números viven en
    /// las habilidades; la clase solo dice cómo se llama todo y cómo se ve.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Clase", fileName = "Clase")]
    public sealed class ClassDefinition : ScriptableObject
    {
        public string displayName = "Clase";
        [Tooltip("Nombre con la facción Pacto Oscuro (vacío = el mismo). Templario → Juramentado.")]
        public string mirrorName = "";
        [TextArea] public string description;

        [Header("Recurso")]
        public string resourceName = "Rabia";
        public string resourceMirrorName = "";
        [Min(1f)] public float resourceMax = 100f;
        [Min(0f)] public float resourceStart = 0f;
        [Min(0f)] public float resourceGainPerHit = 5f;
        [Min(0f)] public float resourceGainPerDamageTaken = 0.4f;
        [Tooltip("Cambio por segundo fuera de combate (negativo baja, positivo sube).")]
        public float resourceOutOfCombatChange = -8f;

        [Header("Especializaciones")]
        public SpecializationDefinition[] specializations = new SpecializationDefinition[2];

        [Header("Aspecto")]
        public Color bodyColor = new Color(0.25f, 0.55f, 0.95f);
        public Color mirrorBodyColor = new Color(0.25f, 0.55f, 0.95f);

        public string DisplayName => FactionSettings.IsPacto && !string.IsNullOrEmpty(mirrorName) ? mirrorName : displayName;
        public string ResourceName => FactionSettings.IsPacto && !string.IsNullOrEmpty(resourceMirrorName) ? resourceMirrorName : resourceName;
        public Color BodyColor => FactionSettings.IsPacto ? mirrorBodyColor : bodyColor;
    }
}
