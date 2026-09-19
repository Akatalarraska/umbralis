using UnityEngine;
using UnityEngine.UI;
using Umbralis.Combat;
using Umbralis.Passives;

namespace Umbralis.HUD
{
    /// <summary>
    /// Enlaza la vida, el recurso y la pasiva del jugador con el HUD (arriba a
    /// la izquierda). Se actualiza cada frame: son dos barras y un texto, no
    /// merece la pena suscribirse a eventos.
    /// </summary>
    public sealed class PlayerStatusHud : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private ClassResource resource;
        [SerializeField] private HudBar healthBar;
        [SerializeField] private HudBar resourceBar;
        [Tooltip("Texto para la pasiva activa: cargas de Muralla, Sed de batalla encendida...")]
        [SerializeField] private Text passiveLabel;

        private void Update()
        {
            if (health != null && healthBar != null)
                healthBar.Set(health.Fraction, $"{Mathf.CeilToInt(health.Current)} / {Mathf.CeilToInt(health.Max)}");

            if (resource != null && resourceBar != null)
                resourceBar.Set(resource.Fraction, $"{resource.DisplayName} {Mathf.FloorToInt(resource.Current)}");

            if (passiveLabel != null && health != null) passiveLabel.text = DescribePassive(health.gameObject);
        }

        /// <summary>La pasiva cambia con la especialización, así que se busca cada vez (es un solo GetComponent).</summary>
        private static string DescribePassive(GameObject player)
        {
            BulwarkPassive bulwark = player.GetComponent<BulwarkPassive>();
            if (bulwark != null) return $"Muralla  {bulwark.Charges} / {bulwark.maxCharges}";

            BattleThirstPassive thirst = player.GetComponent<BattleThirstPassive>();
            if (thirst != null) return thirst.IsActive ? "Sed de batalla ACTIVA" : "Sed de batalla";

            return string.Empty;
        }
    }
}
