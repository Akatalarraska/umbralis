using System.Collections;
using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Muñeco de entrenamiento: parpadea al recibir daño, sale despedido en la
    /// dirección del golpe y reaparece en su sitio unos segundos después de morir.
    /// Sirve para sentir el impacto de las habilidades sin IA de por medio.
    /// </summary>
    [RequireComponent(typeof(Health), typeof(CharacterController))]
    public sealed class TrainingDummy : MonoBehaviour
    {
        [SerializeField] private Renderer body;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material hitMaterial;

        [Tooltip("Metros que retrocede por cada 10 puntos de daño.")]
        [SerializeField, Min(0f)] private float knockbackPerTenDamage = 0.6f;
        [SerializeField, Min(0.1f)] private float knockbackDuration = 0.15f;
        [SerializeField, Min(0.5f)] private float respawnDelay = 2f;

        private Health health;
        private CharacterController controller;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Vector3 knockbackVelocity;
        private float knockbackTimeLeft;
        private Coroutine flash;

        private void Awake()
        {
            health = GetComponent<Health>();
            controller = GetComponent<CharacterController>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        private void OnEnable()
        {
            health.Damaged += OnDamaged;
            health.Died += OnDied;
        }

        private void OnDisable()
        {
            health.Damaged -= OnDamaged;
            health.Died -= OnDied;
        }

        private void Update()
        {
            if (!controller.enabled) return;

            if (knockbackTimeLeft <= 0f)
            {
                // Gravedad mínima para que se quede pegado al suelo tras un empujón.
                controller.Move(Vector3.down * (5f * Time.deltaTime));
                return;
            }

            knockbackTimeLeft -= Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime);
        }

        private void OnDamaged(float amount, Vector3 direction)
        {
            float distance = knockbackPerTenDamage * amount / 10f;
            knockbackVelocity = direction * (distance / knockbackDuration);
            knockbackTimeLeft = knockbackDuration;

            if (flash != null) StopCoroutine(flash);
            flash = StartCoroutine(Flash());
        }

        private void OnDied()
        {
            StartCoroutine(Respawn());
        }

        private IEnumerator Flash()
        {
            if (body != null && hitMaterial != null) body.sharedMaterial = hitMaterial;
            yield return new WaitForSeconds(0.08f);
            if (body != null && normalMaterial != null) body.sharedMaterial = normalMaterial;
            flash = null;
        }

        private IEnumerator Respawn()
        {
            // "Muerte": desaparece un momento y vuelve a su sitio con toda la vida.
            controller.enabled = false;
            if (body != null) body.enabled = false;
            yield return new WaitForSeconds(respawnDelay);

            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            if (body != null) body.enabled = true;
            controller.enabled = true;
            knockbackTimeLeft = 0f;
            health.Revive();
        }
    }
}
