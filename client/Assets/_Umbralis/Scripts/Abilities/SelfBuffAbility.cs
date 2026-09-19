using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Potenciación defensiva temporal sobre uno mismo (o sobre los aliados
    /// cercanos si <see cref="radius"/> es mayor que 0): menos daño recibido
    /// (de frente o de todo), espinas que devuelven daño, y no poder bajar de
    /// 1 de vida. Cubre Represalia, Muro de escudo, Última Posición y
    /// Estandarte de guerra. No se apunta.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Potenciación defensiva", fileName = "SelfBuff")]
    public sealed class SelfBuffAbility : AbilityDefinition
    {
        [Header("Potenciación")]
        [Min(0.1f)] public float duration = 6f;
        [Tooltip("0 = solo uno mismo; mayor = también los aliados a esa distancia (Estandarte).")]
        [Min(0f)] public float radius = 0f;
        [Tooltip("Fracción de daño que se evita de todas las direcciones.")]
        [Range(0f, 1f)] public float damageReduction = 0f;
        [Tooltip("Fracción de daño que se evita solo de frente (Muro de escudo).")]
        [Range(0f, 1f)] public float frontalDamageReduction = 0f;
        [Tooltip("Fracción del daño recibido que se devuelve al atacante (Represalia).")]
        [Range(0f, 2f)] public float thorns = 0f;
        [Tooltip("Mientras dura no se puede morir: la vida se queda en 1 (Última Posición).")]
        public bool preventDeath = false;

        public override void Execute(in AbilityContext context)
        {
            Health self = context.Caster.Health;
            Vector3 center = context.Caster.transform.position;
            Team team = context.Caster.Team;

            foreach (Health h in Health.All.ToArray())
            {
                if (h.Team != team || !h.IsAlive) continue;
                if (h != self && (radius <= 0f || (h.transform.position - center).sqrMagnitude > radius * radius)) continue;
                StatusEffects status = h.GetComponent<StatusEffects>();
                if (status == null) continue;
                status.AddModifier(new DefensiveBuff(this, self), duration);
            }

            float fxSize = radius > 0f ? radius * 2f : 2.5f;
            SpawnFx(PrimitiveType.Cylinder, center + Vector3.up * 0.05f, Quaternion.identity, new Vector3(fxSize, 0.03f, fxSize), 0.25f);
        }

        /// <summary>El modificador que se registra en cada afectado; recuerda al lanzador para las espinas.</summary>
        private sealed class DefensiveBuff : IDamageModifier
        {
            private readonly SelfBuffAbility def;
            private readonly Health caster;

            public DefensiveBuff(SelfBuffAbility def, Health caster)
            {
                this.def = def;
                this.caster = caster;
            }

            // Misma habilidad y mismo lanzador = la misma potenciación: se renueva, no se apila.
            public override bool Equals(object obj) => obj is DefensiveBuff other && other.def == def && other.caster == caster;
            public override int GetHashCode() => def.GetHashCode() ^ (caster != null ? caster.GetHashCode() : 0);

            public float ModifyOutgoing(float amount, Health victim, string source) => amount;

            public float ModifyIncoming(float amount, Health attacker, Vector3 hitDirection, Health self)
            {
                float original = amount;
                if (def.damageReduction > 0f) amount *= 1f - def.damageReduction;
                if (def.frontalDamageReduction > 0f && Passives.BulwarkPassive.IsFrontal(attacker, hitDirection, self.transform))
                    amount *= 1f - def.frontalDamageReduction;

                // Espinas: se devuelve una parte de lo que iba a entrar, sin pasar por más modificadores nuestros.
                if (def.thorns > 0f && attacker != null && attacker != self && attacker.IsAlive)
                    attacker.TakeDamage(original * def.thorns, (attacker.transform.position - self.transform.position).normalized, self, def.displayName);

                if (def.preventDeath && amount >= self.Current) amount = Mathf.Max(0f, self.Current - 1f);
                return amount;
            }
        }
    }
}
