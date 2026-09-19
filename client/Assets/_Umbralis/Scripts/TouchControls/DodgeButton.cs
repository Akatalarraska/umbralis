using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Umbralis.HUD;
using Umbralis.Player;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Botón de esquiva: un toque y ya. Dispara al apoyar el dedo (no al soltar)
    /// para que responda al instante, que es lo que se espera de una esquiva.
    /// No tiene modo apuntado; la dirección la decide <see cref="PlayerDodge"/>.
    /// </summary>
    public sealed class DodgeButton : MonoBehaviour, IPointerDownHandler
    {
        [Header("Referencias")]
        [SerializeField] private PlayerDodge dodge;
        [SerializeField] private Image background;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Text label;

        [Header("Presentación")]
        [SerializeField] private string displayName = "Esquiva";
        [SerializeField] private Color buttonColor = new Color(0.3f, 0.85f, 1f);

        private void Start()
        {
            if (background != null) background.color = buttonColor;
            if (label != null) label.text = displayName;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        }

        private void Update()
        {
            if (cooldownOverlay != null && dodge != null)
                cooldownOverlay.fillAmount = dodge.CooldownFraction;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (dodge == null || HudLayoutEditor.IsEditing) return;
            dodge.TryDodge(); // en recarga: el toque se ignora
        }
    }
}
