using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Umbralis.HUD
{
    /// <summary>
    /// Hace que un elemento del HUD (un botón) se pueda mover arrastrándolo y
    /// escalar, solo mientras <see cref="HudLayoutEditor.IsEditing"/>. Fuera del
    /// modo edición no hace nada y los toques llegan al botón normal.
    /// Se identifica por el nombre del GameObject para guardar y cargar su sitio.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class HudEditableElement : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [Tooltip("Contorno que se enciende al seleccionar el elemento (opcional).")]
        [SerializeField] private Outline highlight;

        public const float MinScale = 0.6f;
        public const float MaxScale = 1.6f;

        public string Id => gameObject.name;
        public RectTransform Rect { get; private set; }
        public Vector2 DefaultPosition { get; private set; }
        public float DefaultScale { get; private set; }

        public Vector2 Position
        {
            get => Rect.anchoredPosition;
            set => Rect.anchoredPosition = value;
        }

        public float Scale
        {
            get => Rect.localScale.x;
            set => Rect.localScale = Vector3.one * Mathf.Clamp(value, MinScale, MaxScale);
        }

        public bool Selected
        {
            set { if (highlight != null) highlight.enabled = value; }
        }

        private Canvas canvas;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            DefaultPosition = Rect.anchoredPosition;
            DefaultScale = Rect.localScale.x;
            if (highlight != null) highlight.enabled = false;
        }

        private void OnEnable() => HudLayoutEditor.Register(this);
        private void OnDisable() => HudLayoutEditor.Unregister(this);

        public void ResetToDefault()
        {
            Position = DefaultPosition;
            Scale = DefaultScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!HudLayoutEditor.IsEditing) return;
            HudLayoutEditor.Select(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!HudLayoutEditor.IsEditing) return;
            float scale = canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
            Position += eventData.delta / scale;
        }
    }
}
