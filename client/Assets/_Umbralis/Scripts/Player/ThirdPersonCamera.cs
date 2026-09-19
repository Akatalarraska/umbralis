using UnityEngine;
using Umbralis.TouchControls;

namespace Umbralis.Player
{
    /// <summary>
    /// Cámara orbital en tercera persona controlada arrastrando el dedo en la
    /// <see cref="CameraLookZone"/>. Gira alrededor de un punto sobre el jugador
    /// y se acerca si hay un obstáculo entre ambos para no atravesar paredes.
    ///
    /// La sensibilidad se define en grados por "ancho de pantalla" recorrido, de
    /// forma que un mismo gesto gire lo mismo en cualquier móvil sin depender
    /// de la densidad de píxeles.
    /// </summary>
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform target;
        [SerializeField] private CameraLookZone lookZone;
        [SerializeField] private PinchZoom pinchZoom;

        [Header("Órbita")]
        [Tooltip("Punto alrededor del que orbita la cámara, relativo al target (altura de los hombros).")]
        [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.5f, 0f);

        [Tooltip("Distancia deseada entre el pivote y la cámara.")]
        [SerializeField, Min(0.5f)] private float distance = 6f;

        [Header("Zoom (pellizco)")]
        [SerializeField] private Vector2 distanceLimits = new Vector2(2.5f, 12f);
        [Tooltip("Metros que cambia la distancia al separar los dedos un ancho de pantalla.")]
        [SerializeField, Min(0.1f)] private float metersPerScreenWidth = 12f;

        [Tooltip("Grados que gira la cámara al arrastrar el dedo de un lado a otro de la pantalla.")]
        [SerializeField, Min(1f)] private float degreesPerScreenWidth = 240f;

        [Tooltip("Inclinación mínima y máxima en grados (negativo = mirar hacia arriba).")]
        [SerializeField] private Vector2 pitchLimits = new Vector2(-15f, 60f);

        [SerializeField] private float startPitch = 20f;

        [Header("Colisión")]
        [Tooltip("Capas que empujan la cámara hacia el jugador. El propio jugador se ignora siempre.")]
        [SerializeField] private LayerMask collisionMask = ~0;

        [SerializeField, Min(0.05f)] private float collisionRadius = 0.25f;
        [SerializeField, Min(0.1f)] private float minDistance = 1f;

        /// <summary>Ángulo horizontal actual de la cámara, útil para otros sistemas.</summary>
        public float Yaw => yaw;

        private float yaw;
        private float pitch;
        private readonly RaycastHit[] hits = new RaycastHit[8];

        private void Start()
        {
            // Arrancamos mirando en la misma dirección que el jugador.
            yaw = target != null ? target.eulerAngles.y : 0f;
            pitch = Mathf.Clamp(startPitch, pitchLimits.x, pitchLimits.y);
        }

        // LateUpdate para movernos después de que el jugador ya se haya desplazado
        // en este frame; así la cámara nunca va un frame por detrás.
        private void LateUpdate()
        {
            if (target == null) return;

            ApplyLookInput();

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pivot = target.position + pivotOffset;
            Vector3 backward = rotation * Vector3.back;
            float finalDistance = ResolveDistance(pivot, backward);

            transform.SetPositionAndRotation(pivot + backward * finalDistance, rotation);
        }

        private void ApplyLookInput()
        {
            if (lookZone == null) return;

            if (pinchZoom != null)
            {
                float pinch = pinchZoom.ConsumeDelta();
                if (pinch != 0f) // separar los dedos = acercar la cámara
                    distance = Mathf.Clamp(distance - pinch / Screen.width * metersPerScreenWidth, distanceLimits.x, distanceLimits.y);
            }

            Vector2 delta = lookZone.ConsumeDelta();
            if (delta == Vector2.zero) return;

            // Usamos el ancho para ambos ejes: misma velocidad angular por píxel.
            float degreesPerPixel = degreesPerScreenWidth / Screen.width;
            yaw += delta.x * degreesPerPixel;
            pitch -= delta.y * degreesPerPixel; // arrastrar hacia arriba = mirar hacia arriba
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
        }

        /// <summary>
        /// Lanza una esfera desde el pivote hacia atrás y devuelve la distancia
        /// hasta el primer obstáculo que no sea el propio jugador.
        /// </summary>
        private float ResolveDistance(Vector3 pivot, Vector3 direction)
        {
            int count = Physics.SphereCastNonAlloc(
                pivot, collisionRadius, direction, hits, distance, collisionMask, QueryTriggerInteraction.Ignore);

            float best = distance;
            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform == target || hit.transform.IsChildOf(target)) continue;
                if (hit.distance < best) best = hit.distance;
            }

            return Mathf.Max(best, minDistance);
        }
    }
}
