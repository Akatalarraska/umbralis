using UnityEngine;
using UnityEngine.UI;
using Umbralis.Abilities;
using Umbralis.Player;

namespace Umbralis.HUD
{
    /// <summary>
    /// Botón que muestra la especialización activa y cambia a la siguiente al
    /// tocarlo. En combate se apaga (el cambio solo es posible fuera). Al
    /// cambiar, carga el perfil de disposición del HUD de esa especialización.
    /// </summary>
    public sealed class SpecializationHud : MonoBehaviour
    {
        [SerializeField] private SpecializationSwitcher switcher;
        [SerializeField] private Button button;
        [SerializeField] private Text label;
        [SerializeField] private Image background;

        private void Awake()
        {
            if (button != null && switcher != null) button.onClick.AddListener(() => switcher.CycleNext());
        }

        private void OnEnable()
        {
            if (switcher != null) switcher.Changed += OnChanged;
            OnChanged(switcher != null ? switcher.Active : null);
        }

        private void OnDisable()
        {
            if (switcher != null) switcher.Changed -= OnChanged;
        }

        private void Update()
        {
            if (switcher == null) return;
            bool can = switcher.CanSwitch;
            if (button != null) button.interactable = can;
            if (background != null) background.color = can ? new Color(1f, 1f, 1f, 0.85f) : new Color(0.5f, 0.5f, 0.5f, 0.6f);
        }

        private void OnChanged(SpecializationDefinition spec)
        {
            if (label != null) label.text = spec != null ? spec.displayName : "—";
            HudLayoutEditor.SetProfile(spec != null ? spec.name : "default");
        }
    }
}
