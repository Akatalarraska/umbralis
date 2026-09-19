using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Hace visibles los efectos de estado sobre la cabeza: un cubo amarillo
    /// girando mientras está aturdido y uno azul mientras está ralentizado.
    /// Sin esto el jugador no sabría si su control ha entrado o el objetivo
    /// era inmune (regla del diseño: todo efecto se ve).
    /// </summary>
    [RequireComponent(typeof(StatusEffects))]
    public sealed class StatusIndicator : MonoBehaviour
    {
        [SerializeField] private Transform stunMarker;
        [SerializeField] private Transform slowMarker;

        private StatusEffects status;

        private void Awake() => status = GetComponent<StatusEffects>();

        private void LateUpdate()
        {
            Toggle(stunMarker, status.IsStunned, 360f);
            Toggle(slowMarker, status.SpeedMultiplier < 1f, 120f);
        }

        private static void Toggle(Transform marker, bool visible, float spinSpeed)
        {
            if (marker == null) return;
            if (marker.gameObject.activeSelf != visible) marker.gameObject.SetActive(visible);
            if (visible) marker.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        }
    }
}
