using UnityEngine;
using UnityEngine.EventSystems;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Joystick virtual flotante: aparece justo donde el jugador apoya el dedo
    /// dentro de su zona (la mitad izquierda de la pantalla) y desaparece al soltar.
    ///
    /// Se apoya en el EventSystem de Unity UI (IPointerDown/Drag/Up), así que
    /// funciona igual con el Input Manager clásico y con el Input System nuevo,
    /// y soporta varios dedos a la vez: solo escucha al dedo que lo activó.
    ///
    /// Debe ir en un RectTransform que cubra la zona táctil y tenga un Graphic
    /// (por ejemplo una Image transparente con Raycast Target activado).
    /// </summary>
    public sealed class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Referencias")]
        [Tooltip("Círculo grande. Se recoloca en el punto donde se apoya el dedo.")]
        [SerializeField] private RectTransform background;

        [Tooltip("Círculo pequeño que sigue al dedo. Debe ser hijo del background.")]
        [SerializeField] private RectTransform handle;

        [Header("Ajustes")]
        [Tooltip("Radio máximo de desplazamiento del handle, en unidades del Canvas.")]
        [SerializeField, Min(1f)] private float radius = 120f;

        [Tooltip("Fracción del radio que se ignora para evitar movimientos accidentales.")]
        [SerializeField, Range(0f, 0.5f)] private float deadZone = 0.1f;

        /// <summary>
        /// Dirección actual del joystick. Componentes en [-1, 1] y magnitud en [0, 1],
        /// ya con la zona muerta aplicada. Vector2.zero si no se está tocando.
        /// </summary>
        public Vector2 Direction { get; private set; }

        /// <summary>True mientras haya un dedo controlando el joystick.</summary>
        public bool IsActive => activePointerId != NoPointer;

        private const int NoPointer = int.MinValue;

        private RectTransform zone;      // el RectTransform de este objeto
        private int activePointerId = NoPointer;
        private Vector2 centerLocal;     // centro del joystick en coordenadas locales de la zona

        private void Awake()
        {
            zone = (RectTransform)transform;

            // Garantizamos que las posiciones que calculamos se interpreten
            // respecto al centro, independientemente de cómo se montó el prefab.
            CenterAnchors(background);
            CenterAnchors(handle);

            SetVisible(false);
        }

        private void OnDisable()
        {
            // Si el objeto se desactiva con un dedo apoyado, no dejamos el
            // personaje andando solo.
            Release();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Solo un dedo puede controlar el joystick; los demás se ignoran.
            if (IsActive) return;
            if (!ScreenToLocal(eventData, out centerLocal)) return;

            activePointerId = eventData.pointerId;

            // El joystick "nace" bajo el dedo.
            background.localPosition = centerLocal;
            handle.anchoredPosition = Vector2.zero;
            Direction = Vector2.zero;
            SetVisible(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;
            if (!ScreenToLocal(eventData, out Vector2 local)) return;

            Vector2 offset = Vector2.ClampMagnitude(local - centerLocal, radius);
            handle.anchoredPosition = offset;

            // Normalizamos y aplicamos la zona muerta de forma que justo fuera de
            // ella la magnitud sea 0 y en el borde sea 1 (sin saltos bruscos).
            float magnitude = offset.magnitude / radius;
            if (magnitude <= deadZone)
            {
                Direction = Vector2.zero;
            }
            else
            {
                float remapped = (magnitude - deadZone) / (1f - deadZone);
                Direction = offset.normalized * remapped;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;
            Release();
        }

        private void Release()
        {
            activePointerId = NoPointer;
            Direction = Vector2.zero;
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            if (background != null) background.gameObject.SetActive(visible);
        }

        /// <summary>Convierte la posición del puntero a coordenadas locales de la zona.</summary>
        private bool ScreenToLocal(PointerEventData eventData, out Vector2 local)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                zone, eventData.position, eventData.pressEventCamera, out local);
        }

        private static void CenterAnchors(RectTransform rt)
        {
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}
