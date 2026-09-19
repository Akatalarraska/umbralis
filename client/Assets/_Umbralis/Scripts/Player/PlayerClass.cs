using System;
using UnityEngine;
using Umbralis.Abilities;
using Umbralis.Combat;
using Umbralis.Core;

namespace Umbralis.Player
{
    /// <summary>
    /// Clase del personaje (Conquistador, Templario/Juramentado...). Al aplicar
    /// una clase configura el recurso, entrega sus especializaciones al
    /// conmutador y pinta el cuerpo. Cambiar de clase es cosa del prototipo
    /// (para comparar); en el juego real se elige al crear el personaje.
    /// </summary>
    [RequireComponent(typeof(ClassResource), typeof(SpecializationSwitcher))]
    [DefaultExecutionOrder(-10)] // su Start va antes que el del conmutador
    public sealed class PlayerClass : MonoBehaviour
    {
        [SerializeField] private ClassDefinition[] classes = new ClassDefinition[0];
        [SerializeField] private int activeIndex = 0;
        [Tooltip("Renderer del cuerpo, para el color de la clase.")]
        [SerializeField] private Renderer body;

        public ClassDefinition Active => classes != null && activeIndex >= 0 && activeIndex < classes.Length ? classes[activeIndex] : null;
        public int Count => classes != null ? classes.Length : 0;
        public bool CanSwitch => switcher.CanSwitch;

        public event Action<ClassDefinition> Changed;

        private ClassResource resource;
        private SpecializationSwitcher switcher;
        private MaterialPropertyBlock block;

        private void Awake()
        {
            resource = GetComponent<ClassResource>();
            switcher = GetComponent<SpecializationSwitcher>();
        }

        private void OnEnable() => FactionSettings.Changed += OnFactionChanged;
        private void OnDisable() => FactionSettings.Changed -= OnFactionChanged;

        // Antes que SpecializationSwitcher.Start: así arranca ya con las especializaciones de la clase.
        private void Start() => Apply(activeIndex);

        public bool CycleNext()
        {
            if (Count == 0 || !CanSwitch) return false;
            Apply((activeIndex + 1) % Count);
            return true;
        }

        private void Apply(int index)
        {
            if (Count == 0) return;
            activeIndex = Mathf.Clamp(index, 0, Count - 1);
            ClassDefinition c = Active;
            if (c == null) return;

            resource.Configure(c.ResourceName, c.resourceMax, c.resourceStart, c.resourceGainPerHit, c.resourceGainPerDamageTaken, c.resourceOutOfCombatChange);
            switcher.SetSpecializations(c.specializations);
            Paint(c);
            Changed?.Invoke(c);
        }

        /// <summary>Cambio de facción: solo nombres y colores; ni números ni recargas.</summary>
        private void OnFactionChanged(Faction _)
        {
            ClassDefinition c = Active;
            if (c == null) return;
            resource.Rename(c.ResourceName);
            Paint(c);
            Changed?.Invoke(c);
        }

        private void Paint(ClassDefinition c)
        {
            if (body == null) return;
            if (block == null) block = new MaterialPropertyBlock();
            body.GetPropertyBlock(block);
            block.SetColor("_BaseColor", c.BodyColor); // URP Lit
            block.SetColor("_Color", c.BodyColor);     // Standard, por si acaso
            body.SetPropertyBlock(block);
        }
    }
}
