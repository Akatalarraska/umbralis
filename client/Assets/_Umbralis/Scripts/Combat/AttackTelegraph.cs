using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Aviso en el suelo de un ataque enemigo: un contorno con el tamaño de la
    /// zona y un relleno que crece desde el centro hasta llenarla justo cuando
    /// cae el golpe. Es el lenguaje visual de todos los telegrafiados del juego:
    /// "cuando el relleno toca el borde, te pega". Sin sonido todavía.
    /// </summary>
    public sealed class AttackTelegraph : MonoBehaviour
    {
        [SerializeField] private Transform outline;
        [SerializeField] private Transform fill;
        [SerializeField] private float groundOffset = 0.04f;

        private float radius;
        private float duration;
        private float elapsed;
        private bool active;

        private void Awake()
        {
            // Se suelta del enemigo: la zona anunciada no debe moverse aunque a él lo empujen.
            transform.SetParent(null, true);
            Hide();
        }

        /// <summary>Muestra la zona circular en <paramref name="center"/> durante <paramref name="duration"/> segundos.</summary>
        public void Show(Vector3 center, float radius, float duration)
        {
            this.radius = radius;
            this.duration = Mathf.Max(duration, 0.01f);
            elapsed = 0f;
            active = true;

            center.y = groundOffset;
            transform.position = center;
            if (outline != null)
            {
                outline.gameObject.SetActive(true);
                outline.localScale = new Vector3(radius * 2f, 0.01f, radius * 2f);
            }
            if (fill != null) fill.gameObject.SetActive(true);
            UpdateFill();
        }

        public void Hide()
        {
            active = false;
            if (outline != null) outline.gameObject.SetActive(false);
            if (fill != null) fill.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!active) return;
            elapsed += Time.deltaTime;
            UpdateFill();
        }

        private void UpdateFill()
        {
            if (fill == null) return;
            float t = Mathf.Clamp01(elapsed / duration);
            float r = Mathf.Max(radius * t, 0.05f);
            fill.localScale = new Vector3(r * 2f, 0.012f, r * 2f);
            fill.localPosition = new Vector3(0f, 0.005f, 0f); // un pelo por encima del contorno
        }
    }
}
