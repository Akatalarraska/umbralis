using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Umbralis.Abilities;
using Umbralis.Combat;
using Umbralis.HUD;

namespace Umbralis.TouchControls
{
    /// <summary>
    /// Botón de habilidad al estilo MOBA/Brawl Stars:
    /// - Toque corto: lanza al instante auto-apuntando al enemigo más cercano.
    /// - Mantener y arrastrar: entra en modo apuntado; la dirección/punto se
    ///   calcula respecto a la cámara y se muestra con el <see cref="AimIndicator"/>.
    ///   Soltar lanza; volver a arrastrar el dedo hasta el botón cancela.
    /// Mientras se apunta, el joystick de la izquierda sigue moviendo al personaje.
    /// </summary>
    public sealed class AbilityButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Referencias")]
        [SerializeField] private AbilityCaster caster;
        [SerializeField] private int slot;
        [SerializeField] private AimIndicator aimIndicator;
        [SerializeField] private Image background;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Text label;

        [Header("Gesto (unidades del Canvas)")]
        [Tooltip("Desplazamiento a partir del cual el toque pasa a ser apuntado.")]
        [SerializeField, Min(1f)] private float aimThreshold = 25f;

        [Tooltip("Si el dedo vuelve a estar a menos de esto del inicio, soltar cancela.")]
        [SerializeField, Min(1f)] private float cancelRadius = 45f;

        [Tooltip("Desplazamiento que corresponde al alcance máximo (modo Punto).")]
        [SerializeField, Min(10f)] private float maxDrag = 160f;

        private const int NoPointer = int.MinValue;

        private Canvas canvas;
        private Transform cameraTransform;
        private int activePointerId = NoPointer;
        private Vector2 pressPosition;
        private bool aiming;
        private bool cancelled;
        private Vector3 aimDirection;
        private Vector3 aimPoint;
        private Color baseColor = Color.white;

        private AbilityDefinition Ability => caster != null ? caster.GetAbility(slot) : null;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            if (caster != null) caster.SlotsChanged += Refresh;
            Core.FactionSettings.Changed += OnFactionChanged;
            Refresh();
        }

        private void OnDestroy()
        {
            if (caster != null) caster.SlotsChanged -= Refresh;
            Core.FactionSettings.Changed -= OnFactionChanged;
        }

        private void OnFactionChanged(Core.Faction _) => Refresh();

        /// <summary>Nombre y color según la habilidad que haya ahora en la ranura.</summary>
        private void Refresh()
        {
            EndGesture();
            AbilityDefinition ability = Ability;
            if (ability != null)
            {
                baseColor = ability.ButtonColor;
                if (label != null) label.text = ability.DisplayName;
            }
            else
            {
                // Ranura vacía: se ve, para poder colocarla, pero apagada.
                baseColor = new Color(1f, 1f, 1f, 0.25f);
                if (label != null) label.text = string.Empty;
            }
            if (background != null) background.color = baseColor;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        }

        private void Update()
        {
            if (caster == null || Ability == null) return;
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = caster.CooldownFraction(slot);

            // Sin recurso suficiente: el botón se apaga (mientras no se esté apuntando con él).
            if (background != null && !aiming)
                background.color = caster.CanAfford(slot) ? baseColor : baseColor * new Color(0.45f, 0.45f, 0.45f, 1f);
        }

        private void OnDisable() => EndGesture();

        public void OnPointerDown(PointerEventData eventData)
        {
            if (HudLayoutEditor.IsEditing) return; // en modo edición el toque es para mover el botón
            if (activePointerId != NoPointer || caster == null || Ability == null) return;
            if (!caster.IsReady(slot) || !caster.CanAfford(slot)) return; // en enfriamiento o sin recurso: el toque se ignora

            activePointerId = eventData.pointerId;
            pressPosition = eventData.position;
            aiming = false;
            cancelled = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;

            Vector2 drag = (eventData.position - pressPosition) / CanvasScale();
            float magnitude = drag.magnitude;

            if (!aiming && magnitude >= aimThreshold)
            {
                aiming = true;
                transform.localScale = Vector3.one * 1.15f;
            }
            if (!aiming) return;

            cancelled = magnitude < cancelRadius;
            if (background != null) background.color = cancelled ? Color.red : baseColor;

            if (cancelled)
            {
                if (aimIndicator != null) aimIndicator.Hide();
                return;
            }

            ComputeAim(drag);
            ShowIndicator();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;

            if (!aiming)
                caster.TryCastAuto(slot);
            else if (!cancelled)
                caster.TryCast(slot, aimDirection, aimPoint);

            EndGesture();
        }

        /// <summary>Arrastre del pulgar → dirección en el mundo relativa a la cámara.</summary>
        private void ComputeAim(Vector2 drag)
        {
            AbilityDefinition ability = Ability;
            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
            if (cameraTransform != null)
            {
                forward = cameraTransform.forward;
                forward.y = 0f;
                forward.Normalize();
                right = Vector3.Cross(Vector3.up, forward);
            }

            Vector3 world = forward * drag.y + right * drag.x;
            aimDirection = world.sqrMagnitude > 0.0001f ? world.normalized : caster.transform.forward;

            float reach = Mathf.Clamp01(drag.magnitude / maxDrag) * ability.range;
            aimPoint = caster.transform.position + aimDirection * reach;
        }

        private void ShowIndicator()
        {
            if (aimIndicator == null) return;
            AbilityDefinition ability = Ability;

            if (ability.aimMode == AimMode.Point)
                aimIndicator.ShowPoint(aimPoint, ability.AimRadius);
            else
                aimIndicator.ShowDirection(caster.transform.position, aimDirection, ability.range);
        }

        private void EndGesture()
        {
            activePointerId = NoPointer;
            aiming = false;
            cancelled = false;
            transform.localScale = Vector3.one;
            if (background != null) background.color = baseColor;
            if (aimIndicator != null) aimIndicator.Hide();
        }

        private float CanvasScale()
        {
            return canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
        }
    }
}
