using UnityEngine;

namespace Umbralis.Player
{
    /// <summary>
    /// Esquiva del personaje (para el Conquistador, la Embestida): un avance
    /// corto y rápido con su propia recarga, independiente de las habilidades
    /// de la barra. No hace daño ni se apunta: va hacia donde empuja el joystick
    /// o, si está quieto, hacia donde mira el personaje.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    public sealed class PlayerDodge : MonoBehaviour
    {
        [Header("Esquiva")]
        [Tooltip("Metros que avanza.")]
        [SerializeField, Min(0.5f)] private float distance = 4f;

        [Tooltip("Segundos que tarda en recorrerlos.")]
        [SerializeField, Min(0.05f)] private float duration = 0.18f;

        [Tooltip("Segundos hasta poder esquivar otra vez.")]
        [SerializeField, Min(0f)] private float cooldown = 3f;

        public float Cooldown => cooldown;
        public bool IsReady => isActiveAndEnabled && Time.time >= cooldownEnd && !movement.IsDashing && !movement.IsLeaping
            && !movement.IsStunned && !movement.Rooted;

        /// <summary>0 = lista; 1 = acaba de usarse. Para el relleno del botón.</summary>
        public float CooldownFraction
        {
            get
            {
                if (cooldown <= 0f) return 0f;
                float remaining = cooldownEnd - Time.time;
                return remaining <= 0f ? 0f : Mathf.Clamp01(remaining / cooldown);
            }
        }

        private PlayerMovement movement;
        private float cooldownEnd;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
        }

        /// <summary>Intenta esquivar. Devuelve false si está en recarga o ya embistiendo.</summary>
        public bool TryDodge()
        {
            if (!IsReady) return false;

            Vector3 direction = movement.IsMoving ? movement.MoveDirection : transform.forward;
            cooldownEnd = Time.time + cooldown;
            movement.Dash(direction, distance, duration);
            return true;
        }
    }
}
