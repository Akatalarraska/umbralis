using UnityEngine;
using UnityEngine.UI;
using Umbralis.Combat;

namespace Umbralis.HUD
{
    /// <summary>
    /// Panel del objetivo actual, arriba en el centro: nombre y vida. Se oculta
    /// si no hay objetivo. El botón "Cambiar" pasa al siguiente enemigo.
    /// </summary>
    public sealed class TargetHud : MonoBehaviour
    {
        [SerializeField] private TargetSelector selector;
        [SerializeField] private GameObject panel;
        [SerializeField] private HudBar healthBar;
        [SerializeField] private Button cycleButton;

        private void Awake()
        {
            if (cycleButton != null && selector != null)
                cycleButton.onClick.AddListener(selector.CycleNext);
        }

        private void Update()
        {
            bool visible = selector != null && selector.HasTarget;
            if (panel != null && panel.activeSelf != visible) panel.SetActive(visible);
            if (!visible || healthBar == null) return;

            Health target = selector.Current;
            healthBar.Set(target.Fraction, $"{target.name}  {Mathf.CeilToInt(target.Current)} / {Mathf.CeilToInt(target.Max)}");
        }
    }
}
