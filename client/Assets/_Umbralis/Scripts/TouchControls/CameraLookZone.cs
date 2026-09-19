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
        /// <summary>True mientras haya un dedo apoyado en la zona.</summary>
        public bool IsDragging => activePointerId != NoPointer;

        private const int NoPointer = int.MinValue;
        private int activePointerId = NoPointer;
        private Vector2 accumulatedDelta; // en píxeles de pantalla
        private PinchZoom pinch;          // opcional: con dos dedos no se gira

        private void Awake() => pinch = GetComponent<PinchZoom>();

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
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;
            if (pinch != null && pinch.IsPinching) return;
            accumulatedDelta += eventData.delta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;
            activePointerId = NoPointer;
        }
    }
}
