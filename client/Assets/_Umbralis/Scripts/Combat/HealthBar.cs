using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Barra de vida de mundo hecha con dos cubos (fondo y relleno) que siempre
    /// mira a la cámara. Sin canvas: suficiente para el prototipo y barata.
    /// </summary>
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Transform fill;
        [SerializeField, Min(0.1f)] private float width = 1.2f;

        private Transform cam;

        private void LateUpdate()
        {
            if (cam == null && Camera.main != null) cam = Camera.main.transform;
            if (cam != null)
                transform.rotation = Quaternion.LookRotation(transform.position - cam.position, Vector3.up);

            if (health == null || fill == null) return;

            float fraction = Mathf.Clamp01(health.Fraction);
            // El relleno está anclado a la izquierda: escala y desplaza a la vez.
            Vector3 scale = fill.localScale;
            scale.x = width * fraction;
            fill.localScale = scale;
            fill.localPosition = new Vector3(-width * 0.5f + scale.x * 0.5f, 0f, -0.01f);
        }
    }
}
