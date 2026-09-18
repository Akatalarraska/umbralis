using UnityEngine;
using Umbralis.TouchControls;

namespace Umbralis.Player
{
    /// <summary>
    /// Movimiento en tercera persona con CharacterController.
    /// Lee el joystick flotante y traduce su dirección al plano horizontal
    /// relativo a la cámara: empujar "arriba" siempre aleja al personaje de la
    /// cámara, que es lo que el pulgar espera.
    /// El personaje gira hacia donde se mueve y la velocidad escala con lo que
    /// se inclina el joystick (andar suave en el centro, correr en el borde).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private FloatingJoystick joystick;

        [Tooltip("Transform de la cámara que define hacia dónde es 'adelante'. Si está vacío usa Camera.main.")]
        [SerializeField] private Transform cameraTransform;

        [Header("Movimiento")]
        [Tooltip("Velocidad máxima en m/s con el joystick al límite.")]
        [SerializeField, Min(0f)] private float moveSpeed = 6f;

        [Tooltip("Velocidad de giro en grados/segundo hacia la dirección de movimiento.")]
        [SerializeField, Min(0f)] private float turnSpeed = 720f;

        [Tooltip("Gravedad aplicada (negativa). Solo mantiene al personaje pegado al suelo.")]
        [SerializeField] private float gravity = -25f;

        /// <summary>Dirección horizontal de movimiento actual (magnitud 0..1).</summary>
        public Vector3 MoveDirection { get; private set; }

        /// <summary>True si el jugador está empujando el joystick.</summary>
        public bool IsMoving => MoveDirection.sqrMagnitude > 0.0001f;

        private CharacterController controller;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Vector2 input = joystick != null ? joystick.Direction : Vector2.zero;
            MoveDirection = InputToWorld(input);

            // Giro suave hacia la dirección de movimiento.
            if (IsMoving)
            {
                Quaternion targetRotation = Quaternion.LookRotation(MoveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            // Gravedad sencilla: si tocamos suelo mantenemos un pequeño empuje hacia
            // abajo para que isGrounded siga siendo fiable en rampas.
            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            else
                verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = MoveDirection * moveSpeed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Convierte el vector 2D del joystick en un vector 3D horizontal relativo
        /// a la orientación de la cámara (ignorando su inclinación).
        /// </summary>
        private Vector3 InputToWorld(Vector2 input)
        {
            if (input.sqrMagnitude < 0.0001f) return Vector3.zero;

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (cameraTransform != null)
            {
                forward = cameraTransform.forward;
                forward.y = 0f;
                forward.Normalize();
                right = Vector3.Cross(Vector3.up, forward);
            }

            Vector3 world = forward * input.y + right * input.x;

            // La magnitud del joystick ya está en [0,1]; solo evitamos superar 1
            // en las diagonales por errores de redondeo.
            return Vector3.ClampMagnitude(world, 1f);
        }
    }
}
