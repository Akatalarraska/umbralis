using System;
using UnityEngine;
using Umbralis.Abilities;
using Umbralis.Combat;

namespace Umbralis.Player
{
    /// <summary>
    /// Especializaciones del personaje y cuál está activa. Cambiar es gratis
    /// pero solo fuera de combate: sustituye la barra, quita la pasiva anterior
    /// y pone la nueva. La disposición de botones es propia de cada
    /// especialización (perfil del HUD con su nombre).
    /// </summary>
    [RequireComponent(typeof(AbilityCaster))]
    public sealed class SpecializationSwitcher : MonoBehaviour
    {
        [SerializeField] private SpecializationDefinition[] specializations = new SpecializationDefinition[2];
        [SerializeField] private int activeIndex = 0;

        public SpecializationDefinition Active =>
            specializations != null && activeIndex >= 0 && activeIndex < specializations.Length ? specializations[activeIndex] : null;
        public int Count => specializations != null ? specializations.Length : 0;
        public int ActiveIndex => activeIndex;

        /// <summary>Solo fuera de combate (lo dice el recurso de clase; sin recurso, siempre).</summary>
        public bool CanSwitch => resource == null || !resource.InCombat;

        public event Action<SpecializationDefinition> Changed;

        private AbilityCaster caster;
        private ClassResource resource;
        private Behaviour attachedPassive;
        private PassiveDefinition attachedPassiveDefinition;

        private void Awake()
        {
            caster = GetComponent<AbilityCaster>();
            resource = GetComponent<ClassResource>();
        }

        private void Start() => Apply(activeIndex, force: true);

        /// <summary>Pasa a la siguiente especialización. Devuelve false si está en combate.</summary>
        public bool CycleNext()
        {
            if (Count == 0) return false;
            return TrySwitch((activeIndex + 1) % Count);
        }

        public bool TrySwitch(int index)
        {
            if (!CanSwitch || index == activeIndex) return false;
            Apply(index, force: false);
            return true;
        }

        private void Apply(int index, bool force)
        {
            if (Count == 0) return;
            index = Mathf.Clamp(index, 0, Count - 1);
            if (!force && index == activeIndex) return;
            activeIndex = index;
            SpecializationDefinition spec = Active;

            // Pasiva: fuera la anterior, dentro la nueva.
            if (attachedPassive != null && attachedPassiveDefinition != null) attachedPassiveDefinition.Detach(attachedPassive);
            attachedPassive = null;
            attachedPassiveDefinition = null;
            if (spec != null && spec.passive != null)
            {
                attachedPassive = spec.passive.Attach(gameObject, spec);
                attachedPassiveDefinition = spec.passive;
            }

            caster.SetSlots(spec != null ? spec.BuildSlots() : null);
            Changed?.Invoke(spec);
        }
    }
}
