using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Marcadores en el suelo mientras se apunta una habilidad: una franja desde
    /// el jugador en la dirección elegida, o un disco en el punto elegido.
    /// Lo enciende y apaga el botón de habilidad que está apuntando.
    /// </summary>
    public sealed class AimIndicator : MonoBehaviour
    {
        [SerializeField] private Transform directionStrip;
        [SerializeField] private Transform pointDisc;
        [SerializeField, Min(0.05f)] private float stripWidth = 0.6f;
        [SerializeField] private float groundOffset = 0.05f;

        private void Awake() => Hide();

        public void ShowDirection(Vector3 origin, Vector3 direction, float length)
        {
            if (pointDisc != null) pointDisc.gameObject.SetActive(false);
            if (directionStrip == null) return;

            origin.y = groundOffset;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) direction = Vector3.forward;
            direction.Normalize();

            directionStrip.gameObject.SetActive(true);
            directionStrip.SetPositionAndRotation(origin + direction * (length * 0.5f), Quaternion.LookRotation(direction, Vector3.up));
            directionStrip.localScale = new Vector3(stripWidth, 0.02f, length);
        }

        public void ShowPoint(Vector3 point, float radius)
        {
            if (directionStrip != null) directionStrip.gameObject.SetActive(false);
            if (pointDisc == null) return;

            point.y = groundOffset;
            pointDisc.gameObject.SetActive(true);
            pointDisc.position = point;
            // Cilindro primitivo: altura 2 unidades por defecto, por eso la Y tan pequeña.
            pointDisc.localScale = new Vector3(radius * 2f, 0.01f, radius * 2f);
        }

        public void Hide()
        {
            if (directionStrip != null) directionStrip.gameObject.SetActive(false);
            if (pointDisc != null) pointDisc.gameObject.SetActive(false);
        }
    }
}
