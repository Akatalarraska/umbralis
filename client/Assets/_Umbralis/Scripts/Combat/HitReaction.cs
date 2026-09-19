using System.Collections;
using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Reacción visual al recibir daño: parpadeo blanco y un empujón en la
    /// dirección del golpe. No mueve nada por sí mismo: quien controle el
    /// movimiento (muñeco, IA de enemigo) pide cada frame el desplazamiento
    /// del empujón con <see cref="ConsumeKnockback"/>.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public sealed class HitReaction : MonoBehaviour
    {
        [SerializeField] private Renderer body;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material hitMaterial;
        [Tooltip("Metros que retrocede por cada 10 puntos de daño.")]
        [SerializeField, Min(0f)] private float knockbackPerTenDamage = 0.6f;
        [SerializeField, Min(0.1f)] private float knockbackDuration = 0.15f;

        /// <summary>True mientras dura el empujón.</summary>
        public bool IsKnockedBack => knockbackTimeLeft > 0f;

        private Health health;
        private Vector3 knockbackVelocity;
        private float knockbackTimeLeft;
        private Coroutine flash;

        private void Awake() => health = GetComponent<Health>();
        private void OnEnable() => health.Damaged += OnDamaged;
        private void OnDisable() => health.Damaged -= OnDamaged;

        /// <summary>Desplazamiento del empujón para este frame (cero si no hay).</summary>
        public Vector3 ConsumeKnockback(float deltaTime)
        {
            if (knockbackTimeLeft <= 0f) return Vector3.zero;
            knockbackTimeLeft -= deltaTime;
            return knockbackVelocity * deltaTime;
        }

        public void CancelKnockback() => knockbackTimeLeft = 0f;

        /// <summary>Desplazamiento forzado (atraer, empujar) independiente del daño.</summary>
        public void Push(Vector3 displacement, float duration)
        {
            if (duration <= 0f) return;
            knockbackVelocity = displacement / duration;
            knockbackTimeLeft = duration;
        }

        private void OnDamaged(float amount, Vector3 direction)
        {
            float distance = knockbackPerTenDamage * amount / 10f;
            knockbackVelocity = direction * (distance / knockbackDuration);
            knockbackTimeLeft = knockbackDuration;

            if (flash != null) StopCoroutine(flash);
            flash = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            if (body != null && hitMaterial != null) body.sharedMaterial = hitMaterial;
            yield return new WaitForSeconds(0.08f);
            if (body != null && normalMaterial != null) body.sharedMaterial = normalMaterial;
            flash = null;
        }
    }
}
