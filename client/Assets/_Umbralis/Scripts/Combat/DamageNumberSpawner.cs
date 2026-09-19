using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Escucha todo el daño de la escena y hace aparecer un número flotante
    /// sobre la víctima. Rojo si la víctima es el jugador, blanco si es un
    /// enemigo (y más grande cuanto mayor el golpe). Uno por escena.
    /// </summary>
    public sealed class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Aspecto")]
        [SerializeField] private Font font;
        [SerializeField] private Color enemyHitColor = new Color(1f, 0.95f, 0.7f);
        [SerializeField] private Color playerHitColor = new Color(1f, 0.3f, 0.25f);
        [Tooltip("Tamaño de letra para un golpe de 10 puntos; crece suavemente con el daño.")]
        [SerializeField, Min(0.01f)] private float baseSize = 0.22f;
        [SerializeField, Min(0.1f)] private float lifetime = 0.8f;
        [Tooltip("Altura sobre los pies de la víctima a la que nace el número.")]
        [SerializeField] private float spawnHeight = 2.2f;

        private void OnEnable() => Health.AnyDamaged += OnDamaged;
        private void OnDisable() => Health.AnyDamaged -= OnDamaged;

        private void OnDamaged(Health attacker, Health victim, float amount)
        {
            if (victim == null) return;

            var go = new GameObject("DamageNumber");
            go.transform.position = victim.transform.position + Vector3.up * spawnHeight
                + new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));

            TextMesh text = go.AddComponent<TextMesh>();
            text.font = font;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 48;
            text.fontStyle = FontStyle.Bold;
            if (font != null) go.GetComponent<MeshRenderer>().sharedMaterial = font.material;

            bool hitPlayer = victim.Team == Team.Player;
            // Escala con el daño con rendimientos decrecientes para que 100 no sea 10 veces 10.
            float size = baseSize * Mathf.Sqrt(Mathf.Max(amount, 1f) / 10f);
            Vector3 velocity = Vector3.up * 2.2f + new Vector3(Random.Range(-0.6f, 0.6f), 0f, 0f);

            go.AddComponent<DamageNumber>().Show(
                Mathf.RoundToInt(amount).ToString(), hitPlayer ? playerHitColor : enemyHitColor, size, lifetime, velocity);
        }
    }
}
