using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Umbralis.Combat;

namespace Umbralis.HUD
{
    /// <summary>
    /// Panel plegable del medidor de daño: DPS, total, duración y desglose por
    /// habilidad con porcentaje. Un botón "DPS" lo muestra u oculta; otro pone
    /// el medidor a cero. Se refresca unas pocas veces por segundo, no cada frame.
    /// </summary>
    public sealed class DamageMeterHud : MonoBehaviour
    {
        [SerializeField] private DamageMeter meter;
        [SerializeField] private GameObject panel;
        [SerializeField] private Text text;
        [SerializeField] private Button toggleButton;
        [SerializeField] private Button resetButton;
        [SerializeField, Min(1)] private int maxRows = 8;
        [SerializeField, Min(0.05f)] private float refreshInterval = 0.25f;

        private float nextRefresh;
        private readonly StringBuilder sb = new StringBuilder();

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);
            if (toggleButton != null) toggleButton.onClick.AddListener(() => panel.SetActive(!panel.activeSelf));
            if (resetButton != null && meter != null) resetButton.onClick.AddListener(meter.Reset);
        }

        private void Update()
        {
            if (panel == null || !panel.activeSelf || meter == null || text == null) return;
            if (Time.time < nextRefresh) return;
            nextRefresh = Time.time + refreshInterval;

            sb.Clear();
            sb.Append(meter.IsActive ? "<b>DPS</b> " : "<b>DPS</b> (parado) ");
            sb.Append(meter.DamagePerSecond.ToString("0"));
            sb.Append("   total ").Append(meter.Total.ToString("0"));
            sb.Append("   ").Append(meter.Duration.ToString("0.0")).Append(" s\n");

            var rows = meter.Breakdown;
            int count = Mathf.Min(rows.Count, maxRows);
            for (int i = 0; i < count; i++)
            {
                float pct = meter.Total > 0f ? rows[i].Value / meter.Total * 100f : 0f;
                sb.Append(rows[i].Key).Append("  ").Append(rows[i].Value.ToString("0")).Append("  (").Append(pct.ToString("0")).Append(" %)\n");
            }
            if (rows.Count == 0) sb.Append("Pega a algo para empezar a medir.");
            text.text = sb.ToString();
        }
    }
}
