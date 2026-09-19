using UnityEngine;
using Umbralis.Combat;
using Umbralis.HUD;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Une los toques cortos en pantalla (zona de joystick y zona de cámara)
    /// con la selección de objetivo: tocar a un enemigo lo selecciona. Un toque
    /// en el vacío no deselecciona (perder el objetivo por un roce sería peor
    /// que mantenerlo).
    /// </summary>
    public sealed class TapToTarget : MonoBehaviour
    {
        [SerializeField] private TapDetector[] detectors;
        [SerializeField] private TargetSelector selector;

        private void OnEnable()
        {
            foreach (TapDetector d in detectors) if (d != null) d.Tapped += OnTapped;
        }

        private void OnDisable()
        {
            foreach (TapDetector d in detectors) if (d != null) d.Tapped -= OnTapped;
        }

        private void OnTapped(Vector2 screenPosition)
        {
            if (selector == null || HudLayoutEditor.IsEditing) return;
            selector.TrySelectAtScreenPoint(screenPosition);
        }
    }
}
