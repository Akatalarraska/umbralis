using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Umbralis.Combat;
using Umbralis.Enemies;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Lanzar escudo: sale hacia la dirección elegida, golpea al primer enemigo
    /// a tiro y rebota hasta <see cref="maxTargets"/> enemigos cercanos entre sí,
    /// dañando, atrayéndolos hacia el lanzador y sumando amenaza.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Lanzar escudo", fileName = "ShieldThrow")]
    public sealed class ShieldThrowAbility : AbilityDefinition
    {
        [Header("Rebotes")]
        [Min(1)] public int maxTargets = 3;
        [Tooltip("Distancia máxima entre un objetivo y el siguiente rebote.")]
        [Min(1f)] public float bounceRange = 6f;
        [Min(0.02f)] public float bounceDelay = 0.15f;
        [Tooltip("Metros que se acerca cada objetivo al lanzador.")]
        [Min(0f)] public float pullDistance = 2f;
        [Min(0f)] public float threat = 30f;

        public override void Execute(in AbilityContext context)
        {
            Health first = context.Target;
            if (first == null)
            {
                // Sin objetivo dirigido: el primer enemigo en la dirección del lanzamiento.
                Vector3 origin = context.Caster.transform.position;
                float bestDot = 0.5f;
                foreach (Health h in Health.All)
                {
                    if (h.Team == context.Caster.Team || !h.IsAlive) continue;
                    Vector3 to = h.transform.position - origin;
                    to.y = 0f;
                    if (to.sqrMagnitude > range * range) continue;
                    float dot = Vector3.Dot(context.Direction, to.normalized);
                    if (dot > bestDot) { bestDot = dot; first = h; }
                }
            }
            if (first == null)
            {
                SpawnFx(PrimitiveType.Cube, context.Origin + context.Direction * (range * 0.5f), Quaternion.LookRotation(context.Direction, Vector3.up), new Vector3(0.4f, 0.4f, range), 0.15f);
                return;
            }
            context.Caster.StartCoroutine(Bounce(context.Caster, first));
        }

        private IEnumerator Bounce(AbilityCaster caster, Health first)
        {
            var hit = new HashSet<Health>();
            Health current = first;
            Vector3 from = caster.Origin;

            for (int i = 0; i < maxTargets && current != null; i++)
            {
                if (!caster.isActiveAndEnabled) yield break;
                hit.Add(current);
                Vector3 at = current.transform.position + Vector3.up;

                // Trazo del escudo y golpe.
                Vector3 seg = at - from;
                SpawnFx(PrimitiveType.Cube, from + seg * 0.5f, Quaternion.LookRotation(seg.normalized, Vector3.up), new Vector3(0.35f, 0.35f, seg.magnitude), bounceDelay);
                Vector3 toCaster = caster.transform.position - current.transform.position;
                toCaster.y = 0f;
                caster.Health.DealDamage(current, damage, -toCaster.normalized, DisplayName);
                HitReaction reaction = current.GetComponent<HitReaction>();
                if (reaction != null && pullDistance > 0f) reaction.Push(toCaster.normalized * pullDistance, 0.2f);
                EnemyBrain brain = current.GetComponent<EnemyBrain>();
                if (brain != null) brain.AddThreat(caster.Health, threat);

                from = at;
                current = NextTarget(current, hit, caster.Team);
                if (current != null) yield return new WaitForSeconds(bounceDelay);
            }
        }

        private Health NextTarget(Health origin, HashSet<Health> exclude, Team myTeam)
        {
            Health best = null;
            float bestSqr = bounceRange * bounceRange;
            foreach (Health h in Health.All)
            {
                if (h.Team == myTeam || !h.IsAlive || exclude.Contains(h)) continue;
                float sqr = (h.transform.position - origin.transform.position).sqrMagnitude;
                if (sqr < bestSqr) { bestSqr = sqr; best = h; }
            }
            return best;
        }
    }
}
