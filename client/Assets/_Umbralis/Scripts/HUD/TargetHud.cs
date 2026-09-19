using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.HUD
{
    /// <summary>
    /// Panel del objetivo actual, arriba en el centro: nombre y vida. Se oculta
    /// si no hay objetivo. (El botón de cambiar objetivo está en la barra de
    /// combate: <see cref="TouchControls.CycleTargetButton"/>.)
    /// </summary>
    public sealed class TargetHud : MonoBehaviour
    {
        [SerializeField] private TargetSelector selector;
        [SerializeField] private GameObject panel;
        [SerializeField] private HudBar healthBar;

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
