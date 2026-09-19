using System.Collections;
using UnityEngine;
using Umbralis.Combat;
using Umbralis.Enemies;
using Umbralis.Passives;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Golpe cuerpo a cuerpo: daña a todo enemigo dentro de un cono delante del
    /// jugador. Con los efectos opcionales cubre casi todo el kit del Devastador:
    /// Tajo giratorio (360°), Ejecutar (más daño a poca vida), Desgarrar
    /// (sangrado), Ráfaga (varios golpes seguidos), Golpe conmocionador
    /// (aturde) y Patada (interrumpe). Todo se ajusta en el asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Golpe (melé)", fileName = "Melee")]
    public sealed class MeleeAbility : AbilityDefinition
    {
        [Header("Golpe")]
        [Range(10f, 360f)] public float coneDegrees = 100f;
        [Tooltip("Golpes seguidos (Ráfaga). El daño es por golpe.")]
        [Min(1)] public int hitCount = 1;
        [Min(0.05f)] public float hitInterval = 0.2f;

        [Header("Ejecutar (0 = desactivado)")]
        [Tooltip("Fracción de vida del objetivo por debajo de la cual se multiplica el daño.")]
        [Range(0f, 1f)] public float executeThreshold = 0f;
        [Min(1f)] public float executeMultiplier = 3f;

        [Header("Sangrado (0 = desactivado)")]
        [Min(0f)] public float bleedDamagePerTick = 0f;
        [Min(0.1f)] public float bleedInterval = 1f;
        [Min(0f)] public float bleedDuration = 0f;

        [Header("Tanque")]
        [Tooltip("Amenaza extra que suma a cada objetivo golpeado (Tajo de barrido).")]
        [Min(0f)] public float threat = 0f;
        [Tooltip("Aplastar: consume las cargas de Muralla y hace este extra por cada una (0,2 = +20 %).")]
        [Min(0f)] public float bonusPerBulwarkCharge = 0f;

        [Header("Control")]
        [Tooltip("Segundos de aturdimiento (antes de rendimientos decrecientes). 0 = no aturde.")]
        [Min(0f)] public float stunDuration = 0f;
        [Tooltip("Interrumpe lo que el objetivo esté cargando o anunciando.")]
        public bool interrupts = false;

        public override void Execute(in AbilityContext context)
        {
            if (hitCount <= 1) Strike(context);
            else context.Caster.StartCoroutine(Flurry(context));
        }

        private IEnumerator Flurry(AbilityContext context)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (!context.Caster.isActiveAndEnabled) yield break;
                // Cada golpe sale hacia donde mira el personaje en ese momento.
                var now = new AbilityContext(context.Caster, context.Caster.Origin, context.Caster.transform.forward, context.Point, context.Target);
                Strike(now);
                if (i < hitCount - 1) yield return new WaitForSeconds(hitInterval);
            }
        }

        private void Strike(in AbilityContext context)
        {
            Health attacker = context.Caster.Health;
            Vector3 feet = context.Caster.transform.position;
            Vector3 direction = context.Direction;

            // Aplastar: se lleva las cargas de Muralla una vez por lanzamiento, no por objetivo.
            float chargeBonus = 1f;
            if (bonusPerBulwarkCharge > 0f)
            {
                BulwarkPassive bulwark = context.Caster.GetComponent<BulwarkPassive>();
                if (bulwark != null) chargeBonus += bonusPerBulwarkCharge * bulwark.ConsumeCharges();
            }

            // Cada objetivo alcanzado recibe además los efectos opcionales.
            DamageInSphere(feet, range, 0f, attacker, direction, coneDegrees, direction, victim =>
            {
                float amount = damage * chargeBonus;
                if (executeThreshold > 0f && victim.Fraction <= executeThreshold) amount *= executeMultiplier;
                attacker.DealDamage(victim, amount, direction, displayName);

                if (threat > 0f)
                {
                    EnemyBrain brain = victim.GetComponent<EnemyBrain>();
                    if (brain != null) brain.AddThreat(attacker, threat);
                }

                StatusEffects status = victim.GetComponent<StatusEffects>();
                if (status == null) return;
                if (bleedDamagePerTick > 0f && bleedDuration > 0f) status.ApplyBleed(attacker, bleedDamagePerTick, bleedInterval, bleedDuration, displayName + " (sangrado)");
                if (stunDuration > 0f) status.ApplyStun(stunDuration);
                if (interrupts) status.Interrupt();
            });

            // Tajo: una placa fina delante del personaje durante un instante.
            Vector3 pos = context.Origin + direction * (range * 0.5f);
            SpawnFx(PrimitiveType.Cube, pos, Quaternion.LookRotation(direction, Vector3.up),
                new Vector3(range * 0.9f, 0.08f, range), 0.1f);
        }
    }
}
