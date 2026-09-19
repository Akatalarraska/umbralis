using UnityEngine;
using Umbralis.Combat;
using Umbralis.HUD;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Une el toque corto en la zona de cámara con la selección de objetivo:
    /// tocar a un enemigo lo selecciona. Un toque en el vacío no deselecciona
    /// (perder el objetivo por un roce sería peor que mantenerlo).
    /// </summary>
    public sealed class TapToTarget : MonoBehaviour
    {
        [SerializeField] private CameraLookZone lookZone;
        [SerializeField] private TargetSelector selector;

        private void OnEnable()
        {
            if (lookZone != null) lookZone.Tapped += OnTapped;
        }

        private void OnDisable()
        {
            if (lookZone != null) lookZone.Tapped -= OnTapped;
        }

        private void OnTapped(Vector2 screenPosition)
        {
            if (selector == null || HudLayoutEditor.IsEditing) return;
            selector.TrySelectAtScreenPoint(screenPosition);
        }
    }
}
