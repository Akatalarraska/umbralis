using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Umbralis.Combat;
using Umbralis.HUD;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Botón de la barra que pasa al siguiente objetivo. Un toque y ya; vive
    /// junto al pulgar derecho para no tener que subir la mano.
    /// </summary>
    public sealed class CycleTargetButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TargetSelector selector;
        [SerializeField] private Image background;
        [SerializeField] private Text label;

        [SerializeField] private string displayName = "Objetivo";
        [SerializeField] private Color buttonColor = new Color(1f, 0.6f, 0.3f);

        private void Start()
        {
            if (background != null) background.color = buttonColor;
            if (label != null) label.text = displayName;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (selector == null || HudLayoutEditor.IsEditing) return;
            selector.CycleNext();
        }
    }
}
