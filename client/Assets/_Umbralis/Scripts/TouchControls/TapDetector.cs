using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Detecta un "toque" (apoyar y soltar rápido, sin arrastrar) sobre el
    /// RectTransform en el que está. Convive con otros gestos del mismo objeto
    /// (joystick, giro de cámara): solo avisa, no consume nada. El umbral de
    /// desplazamiento se mide en pulgadas para que no dependa de la densidad
    /// de píxeles del móvil.
    /// </summary>
    public sealed class TapDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("Duración máxima del toque en segundos.")]
        [SerializeField, Min(0.05f)] private float maxDuration = 0.3f;
        [Tooltip("Desplazamiento máximo del dedo en pulgadas.")]
        [SerializeField, Min(0.01f)] private float maxDistanceInches = 0.15f;

        /// <summary>Toque válido, con su posición en pantalla.</summary>
        public event Action<Vector2> Tapped;

        private const int NoPointer = int.MinValue;
        private int pointerId = NoPointer;
        private Vector2 pressPosition;
        private float pressTime;

        private float MaxDistancePixels => maxDistanceInches * (Screen.dpi > 0f ? Screen.dpi : 160f);

        private void OnDisable() => pointerId = NoPointer;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (pointerId != NoPointer) return;
            pointerId = eventData.pointerId;
            pressPosition = eventData.position;
            pressTime = Time.unscaledTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != pointerId) return;
            pointerId = NoPointer;

            float maxPixels = MaxDistancePixels;
            bool shortPress = Time.unscaledTime - pressTime <= maxDuration;
            bool noDrag = (eventData.position - pressPosition).sqrMagnitude <= maxPixels * maxPixels;
            if (shortPress && noDrag) Tapped?.Invoke(eventData.position);
        }
    }
}
