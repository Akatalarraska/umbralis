using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Zona táctil (mitad derecha de la pantalla) que acumula el desplazamiento
    /// del dedo para girar la cámara. No mueve nada por sí misma: la cámara le
    /// pide el delta acumulado cada frame con <see cref="ConsumeDelta"/>.
    ///
    /// Escucha solo al primer dedo que toca la zona, de modo que el joystick de
    /// la izquierda y esta zona pueden usarse a la vez sin interferencias.
    /// Los botones de la barra de combate que se apilen por encima capturarán
    /// sus propios toques y no llegarán aquí.
    /// </summary>
    public sealed class CameraLookZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Tooltip("Un toque más corto que esto y con menos desplazamiento cuenta como 'tocar', no como girar.")]
        [SerializeField, Min(0.05f)] private float tapMaxDuration = 0.3f;
        [SerializeField, Min(1f)] private float tapMaxDistance = 20f; // píxeles

        /// <summary>True mientras haya un dedo apoyado en la zona.</summary>
        public bool IsDragging => activePointerId != NoPointer;

        /// <summary>Toque corto sin arrastre, con su posición en pantalla. Sirve para seleccionar objetivo.</summary>
        public event Action<Vector2> Tapped;

        private const int NoPointer = int.MinValue;
        private int activePointerId = NoPointer;
        private Vector2 accumulatedDelta; // en píxeles de pantalla
        private Vector2 pressPosition;
        private float pressTime;

        /// <summary>
        /// Devuelve el desplazamiento acumulado desde la última llamada (en píxeles
        /// de pantalla) y lo pone a cero. Llamar una vez por frame desde la cámara.
        /// </summary>
        public Vector2 ConsumeDelta()
        {
            Vector2 delta = accumulatedDelta;
            accumulatedDelta = Vector2.zero;
            return delta;
        }

        private void OnDisable()
        {
            activePointerId = NoPointer;
            accumulatedDelta = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsDragging) return;
            activePointerId = eventData.pointerId;
            pressPosition = eventData.position;
            pressTime = Time.unscaledTime;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;
            accumulatedDelta += eventData.delta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;
            activePointerId = NoPointer;

            bool shortPress = Time.unscaledTime - pressTime <= tapMaxDuration;
            bool noDrag = (eventData.position - pressPosition).sqrMagnitude <= tapMaxDistance * tapMaxDistance;
            if (shortPress && noDrag) Tapped?.Invoke(eventData.position);
        }
    }
}
