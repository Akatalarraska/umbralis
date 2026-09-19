using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Anillo en el suelo bajo el objetivo seleccionado. Sigue al objetivo
    /// mientras lo haya y se oculta cuando no.
    /// </summary>
    public sealed class TargetMarker : MonoBehaviour
    {
        [SerializeField] private TargetSelector selector;
        [SerializeField] private Transform ring;
        [SerializeField] private float groundOffset = 0.03f;
        [Tooltip("Vueltas por segundo del anillo, para que se vea que está 'vivo'.")]
        [SerializeField] private float spinSpeed = 0.5f;

        private void LateUpdate()
        {
            if (ring == null) return;
            bool visible = selector != null && selector.HasTarget;
            if (ring.gameObject.activeSelf != visible) ring.gameObject.SetActive(visible);
            if (!visible) return;

            Vector3 position = selector.Current.transform.position;
            position.y = groundOffset;
            ring.position = position;
            ring.Rotate(Vector3.up, spinSpeed * 360f * Time.deltaTime, Space.World);
        }
    }
}
