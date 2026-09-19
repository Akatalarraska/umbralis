using UnityEngine;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Carga: el personaje se desplaza rápidamente en la dirección elegida
    /// (hasta <see cref="AbilityDefinition.range"/> metros) y daña a los enemigos
    /// que encuentra por el camino. Tipo genérico (Salto, Carga con escudo...);
    /// la esquiva no usa esto, tiene su propio <c>PlayerDodge</c>.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Carga (dash)", fileName = "Dash")]
    public sealed class DashAbility : AbilityDefinition
    {
        [Header("Carga")]
        [Min(0.05f)] public float duration = 0.2f;
        [Tooltip("Anchura del pasillo que golpea (radio).")]
        [Min(0.1f)] public float hitRadius = 0.8f;

        public override void Execute(in AbilityContext context)
        {
            context.Caster.Movement.Dash(context.Direction, range, duration);

            // El daño se resuelve de golpe a lo largo del recorrido: para el
            // prototipo es indistinguible de hacerlo frame a frame y más robusto.
            Vector3 feet = context.Caster.transform.position;
            int steps = Mathf.CeilToInt(range / hitRadius);
            var alreadyHit = new System.Collections.Generic.HashSet<Combat.Health>();
            for (int i = 0; i <= steps; i++)
            {
                Vector3 center = feet + context.Direction * (range * i / steps);
                foreach (Combat.Health h in Combat.Health.All.ToArray())
                {
                    if (h.Team == context.Caster.Team || !h.IsAlive || alreadyHit.Contains(h)) continue;
                    Vector3 to = h.transform.position - center;
                    to.y = 0f;
                    if (to.sqrMagnitude > hitRadius * hitRadius) continue;
                    alreadyHit.Add(h);
                    context.Caster.Health.DealDamage(h, damage, context.Direction);
                }
            }

            SpawnFx(PrimitiveType.Cube, context.Origin + context.Direction * (range * 0.5f),
                Quaternion.LookRotation(context.Direction, Vector3.up), new Vector3(0.3f, 0.3f, range), duration);
        }
    }
}
