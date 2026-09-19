using UnityEngine;
using Umbralis.Combat;
using Umbralis.Enemies;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Provocación: obliga a los enemigos a atacar al lanzador durante unos
    /// segundos. Con <see cref="radius"/> mayor que 0 afecta a todos los
    /// cercanos (Grito de desafío); si no, al objetivo dirigido a distancia
    /// (Provocar). Además suma amenaza para que sigan pegados después.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Provocación", fileName = "Taunt")]
    public sealed class TauntAbility : AbilityDefinition
    {
        [Header("Provocación")]
        [Min(0.5f)] public float duration = 4f;
        [Tooltip("0 = un solo objetivo (dirigido); mayor = todos los enemigos a esa distancia.")]
        [Min(0f)] public float radius = 0f;
        [Min(0f)] public float threat = 50f;

        public override bool CanExecute(in AbilityContext context)
        {
            return radius > 0f || context.Target != null;
        }

        public override void Execute(in AbilityContext context)
        {
            Health self = context.Caster.Health;
            Vector3 center = context.Caster.transform.position;

            if (radius > 0f)
            {
                foreach (Health h in Health.All.ToArray())
                {
                    if (h.Team == self.Team || !h.IsAlive) continue;
                    if ((h.transform.position - center).sqrMagnitude > radius * radius) continue;
                    Apply(h, self);
                }
                SpawnFx(PrimitiveType.Cylinder, center + Vector3.up * 0.05f, Quaternion.identity, new Vector3(radius * 2f, 0.03f, radius * 2f), 0.25f);
            }
            else
            {
                Apply(context.Target, self);
                SpawnFx(PrimitiveType.Sphere, context.Target.transform.position + Vector3.up * 2.6f, Quaternion.identity, Vector3.one * 0.5f, 0.4f);
            }
        }

        private void Apply(Health enemy, Health self)
        {
            EnemyBrain brain = enemy.GetComponent<EnemyBrain>();
            if (brain == null) return; // los muñecos no tienen a quién atacar
            brain.Taunt(self, duration);
            brain.AddThreat(self, threat);
            if (damage > 0f) self.DealDamage(enemy, damage, (enemy.transform.position - self.transform.position).normalized, DisplayName);
        }
    }
}
