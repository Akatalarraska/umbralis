using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Interceptar: salta hasta el aliado más cercano dentro del alcance y,
    /// durante unos segundos, una parte del daño que reciba ese aliado la
    /// encaja el lanzador en su lugar. Sin aliados a tiro no se lanza.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Interceptar", fileName = "Intercept")]
    public sealed class InterceptAbility : AbilityDefinition
    {
        [Header("Interceptar")]
        [Min(0.1f)] public float leapDuration = 0.4f;
        [Min(0.5f)] public float duration = 6f;
        [Tooltip("Fracción del daño del aliado que se lleva el lanzador.")]
        [Range(0f, 1f)] public float share = 0.5f;

        public override bool CanExecute(in AbilityContext context) => FindAlly(context.Caster) != null;

        public override void Execute(in AbilityContext context)
        {
            AbilityCaster caster = context.Caster;
            Health ally = FindAlly(caster);
            if (ally == null) return;

            // Aterriza justo al lado del aliado, entre él y el frente del lanzador.
            Vector3 toAlly = ally.transform.position - caster.transform.position;
            toAlly.y = 0f;
            Vector3 destination = ally.transform.position - toAlly.normalized * 1.2f;
            caster.Movement.Leap(destination, leapDuration, 1.5f);

            StatusEffects allyStatus = ally.GetComponent<StatusEffects>();
            if (allyStatus != null) allyStatus.AddModifier(new Redirect(this, caster.Health), duration);

            SpawnFx(PrimitiveType.Sphere, ally.transform.position + Vector3.up, Quaternion.identity, Vector3.one * 2f, 0.3f);
        }

        private Health FindAlly(AbilityCaster caster)
        {
            Health best = null;
            float bestSqr = range * range;
            foreach (Health h in Health.All)
            {
                if (h == caster.Health || h.Team != caster.Team || !h.IsAlive) continue;
                float sqr = (h.transform.position - caster.transform.position).sqrMagnitude;
                if (sqr < bestSqr) { bestSqr = sqr; best = h; }
            }
            return best;
        }

        /// <summary>En el aliado: parte del daño entrante va al protector.</summary>
        private sealed class Redirect : IDamageModifier
        {
            private readonly InterceptAbility def;
            private readonly Health protector;

            public Redirect(InterceptAbility def, Health protector)
            {
                this.def = def;
                this.protector = protector;
            }

            public override bool Equals(object obj) => obj is Redirect other && other.def == def && other.protector == protector;
            public override int GetHashCode() => def.GetHashCode() ^ (protector != null ? protector.GetHashCode() : 0);

            public float ModifyOutgoing(float amount, Health victim, string source) => amount;

            public float ModifyIncoming(float amount, Health attacker, Vector3 hitDirection, Health self)
            {
                if (protector == null || !protector.IsAlive) return amount;
                float taken = amount * def.share;
                protector.TakeDamage(taken, hitDirection, attacker, def.DisplayName);
                return amount - taken;
            }
        }
    }
}
