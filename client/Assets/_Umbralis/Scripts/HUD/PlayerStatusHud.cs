using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.HUD
{
    /// <summary>
    /// Enlaza la vida y el recurso del jugador con sus barras del HUD (arriba a
    /// la izquierda). Se actualiza cada frame: son dos barras, no merece la pena
    /// suscribirse a eventos.
    /// </summary>
    public sealed class PlayerStatusHud : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private ClassResource resource;
        [SerializeField] private HudBar healthBar;
        [SerializeField] private HudBar resourceBar;

        private void Update()
        {
            if (health != null && healthBar != null)
                healthBar.Set(health.Fraction, $"{Mathf.CeilToInt(health.Current)} / {Mathf.CeilToInt(health.Max)}");

            if (resource != null && resourceBar != null)
                resourceBar.Set(resource.Fraction, $"{resource.DisplayName} {Mathf.FloorToInt(resource.Current)}");
        }
    }
}
