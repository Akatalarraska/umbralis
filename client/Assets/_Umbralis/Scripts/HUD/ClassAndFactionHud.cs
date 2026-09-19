using UnityEngine;
using UnityEngine.UI;
using Umbralis.Core;
using Umbralis.Player;

namespace Umbralis.HUD
{
    /// <summary>
    /// Dos botones de prototipo: "Clase" (Conquistador ↔ Templario) y
    /// "Facción" (Llama Blanca ↔ Pacto Oscuro). El de clase solo fuera de
    /// combate; el de facción siempre, porque solo cambia nombres y colores.
    /// </summary>
    public sealed class ClassAndFactionHud : MonoBehaviour
    {
        [SerializeField] private PlayerClass playerClass;
        [SerializeField] private Button classButton;
        [SerializeField] private Text classLabel;
        [SerializeField] private Image classBackground;
        [SerializeField] private Button factionButton;
        [SerializeField] private Text factionLabel;

        private void Awake()
        {
            if (classButton != null && playerClass != null) classButton.onClick.AddListener(() => playerClass.CycleNext());
            if (factionButton != null) factionButton.onClick.AddListener(() =>
                FactionSettings.Current = FactionSettings.IsPacto ? Faction.LlamaBlanca : Faction.PactoOscuro);
        }

        private void OnEnable()
        {
            if (playerClass != null) playerClass.Changed += OnClassChanged;
            FactionSettings.Changed += OnFactionChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (playerClass != null) playerClass.Changed -= OnClassChanged;
            FactionSettings.Changed -= OnFactionChanged;
        }

        private void Update()
        {
            if (playerClass == null) return;
            bool can = playerClass.CanSwitch;
            if (classButton != null) classButton.interactable = can;
            if (classBackground != null) classBackground.color = can ? new Color(1f, 1f, 1f, 0.85f) : new Color(0.5f, 0.5f, 0.5f, 0.6f);
        }

        private void OnClassChanged(Abilities.ClassDefinition _) => Refresh();
        private void OnFactionChanged(Faction _) => Refresh();

        private void Refresh()
        {
            if (classLabel != null) classLabel.text = playerClass != null && playerClass.Active != null ? playerClass.Active.DisplayName : "Clase";
            if (factionLabel != null) factionLabel.text = FactionSettings.IsPacto ? "Pacto Oscuro" : "Llama Blanca";
        }
    }
}
