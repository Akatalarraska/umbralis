using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Un número de daño: sube, se desvanece y se destruye. Mira siempre a la
    /// cámara. Lo crea <see cref="DamageNumberSpawner"/>; no lo pongas a mano.
    /// </summary>
    [RequireComponent(typeof(TextMesh))]
    public sealed class DamageNumber : MonoBehaviour
    {
        private TextMesh text;
        private Transform cam;
        private float lifetime;
        private float age;
        private Vector3 velocity;
        private Color color;

        public void Show(string value, Color color, float size, float lifetime, Vector3 velocity)
        {
            text = GetComponent<TextMesh>();
            text.text = value;
            text.characterSize = size;
            this.color = color;
            this.lifetime = lifetime;
            this.velocity = velocity;
            age = 0f;
        }

        private void LateUpdate()
        {
            age += Time.deltaTime;
            if (age >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            // Sube rápido al principio y frena; se apaga en el último tercio.
            transform.position += velocity * Time.deltaTime;
            velocity = Vector3.Lerp(velocity, Vector3.up * 0.3f, Time.deltaTime * 4f);

            float t = age / lifetime;
            Color c = color;
            c.a = t < 0.66f ? 1f : 1f - (t - 0.66f) / 0.34f;
            text.color = c;

            if (cam == null && Camera.main != null) cam = Camera.main.transform;
            if (cam != null) transform.rotation = Quaternion.LookRotation(transform.position - cam.position, Vector3.up);
        }
    }
}
