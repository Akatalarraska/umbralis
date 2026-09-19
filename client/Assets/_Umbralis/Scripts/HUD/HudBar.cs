using UnityEngine;
using UnityEngine.UI;

namespace Umbralis.HUD
{
    /// <summary>
    /// Barra horizontal de HUD: un relleno que se escala de izquierda a derecha
    /// y un texto encima. No sabe qué mide; quien la use le pasa la fracción.
    /// </summary>
    public sealed class HudBar : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private Text label;

        public void Set(float fraction, string text)
        {
            if (fill != null) fill.fillAmount = Mathf.Clamp01(fraction);
            if (label != null) label.text = text;
        }

        public void SetColor(Color color)
        {
            if (fill != null) fill.color = color;
        }
    }
}
