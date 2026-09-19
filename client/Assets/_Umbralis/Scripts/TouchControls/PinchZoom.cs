using UnityEngine;
using UnityEngine.EventSystems;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Pellizco con dos dedos sobre la zona de cámara. Acumula cuánto se
    /// separan o juntan los dedos (en píxeles) y la cámara lo consume cada
    /// frame para acercarse o alejarse. Mientras hay dos dedos, la zona de giro
    /// se queda quieta (<see cref="IsPinching"/>) para no girar sin querer.
    /// </summary>
    public sealed class PinchZoom : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public bool IsPinching => firstId != NoPointer && secondId != NoPointer;

        private const int NoPointer = int.MinValue;
        private int firstId = NoPointer, secondId = NoPointer;
        private Vector2 firstPos, secondPos;
        private float lastDistance;
        private float accumulated; // píxeles: positivo = separar (acercar cámara)

        /// <summary>Cambio de separación acumulado desde la última llamada, en píxeles. Lo pone a cero.</summary>
        public float ConsumeDelta()
        {
            float d = accumulated;
            accumulated = 0f;
            return d;
        }

        private void OnDisable()
        {
            firstId = secondId = NoPointer;
            accumulated = 0f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (firstId == NoPointer) { firstId = eventData.pointerId; firstPos = eventData.position; }
            else if (secondId == NoPointer && eventData.pointerId != firstId) { secondId = eventData.pointerId; secondPos = eventData.position; }
            else return;

            if (IsPinching) lastDistance = Vector2.Distance(firstPos, secondPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == firstId) firstPos = eventData.position;
            else if (eventData.pointerId == secondId) secondPos = eventData.position;
            else return;

            if (!IsPinching) return;
            float distance = Vector2.Distance(firstPos, secondPos);
            accumulated += distance - lastDistance;
            lastDistance = distance;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId == firstId) firstId = NoPointer;
            else if (eventData.pointerId == secondId) secondId = NoPointer;
        }
    }
}
