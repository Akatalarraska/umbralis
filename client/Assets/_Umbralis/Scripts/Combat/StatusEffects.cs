using System;
using System.Collections.Generic;
using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Efectos de estado de un personaje: aturdimiento, ralentización,
    /// sangrados y potenciación. Quien se mueve o lanza (jugador, IA) consulta
    /// aquí si puede. Los controles llevan <b>rendimientos decrecientes</b>:
    /// cada aplicación seguida sobre el mismo objetivo dura la mitad que la
    /// anterior (100 % → 50 % → 25 %) y la cuarta no hace nada; la cuenta se
    /// reinicia tras <see cref="diminishingWindow"/> segundos sin recibir ese
    /// control.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public sealed class StatusEffects : MonoBehaviour
    {
        [Tooltip("Segundos sin recibir un control para que su cuenta de rendimientos decrecientes se reinicie.")]
        [SerializeField, Min(1f)] private float diminishingWindow = 15f;

        // ---- Aturdimiento ----
        public bool IsStunned => Time.time < stunUntil;
        public float StunRemaining => Mathf.Max(0f, stunUntil - Time.time);
        /// <summary>Aplicaciones seguidas de aturdimiento (0..3). 3 = inmune hasta que se reinicie.</summary>
        public int StunDiminishingStage => DiminishingStage(ref stunDr);

        // ---- Ralentización ----
        /// <summary>Multiplicador de velocidad de movimiento (1 = normal).</summary>
        public float SpeedMultiplier => Time.time < slowUntil ? 1f - slowFraction : 1f;

        // ---- Potenciación (Furia) ----
        public float DamageMultiplier => Time.time < buffUntil ? buffDamageMultiplier : 1f;
        public float Lifesteal => Time.time < buffUntil ? buffLifesteal : 0f;
        public bool IsBuffed => Time.time < buffUntil;

        /// <summary>Se dispara al ser aturdido o interrumpido: cancela lo que se estuviera cargando.</summary>
        public event Action Interrupted;
        /// <summary>Se dispara cuando un control no aplica por estar inmune (para avisar en pantalla).</summary>
        public event Action Immune;

        private struct Diminishing
        {
            public int count;      // aplicaciones dentro de la ventana
            public float resetAt;  // momento en que se olvida la cuenta
        }

        private sealed class Bleed
        {
            public Health attacker;
            public string source;
            public float damagePerTick, tickInterval, nextTick, endsAt;
        }

        private Health health;
        private float stunUntil;
        private Diminishing stunDr;
        private float slowUntil, slowFraction;
        private float buffUntil, buffDamageMultiplier = 1f, buffLifesteal;
        private readonly List<Bleed> bleeds = new List<Bleed>();

        private void Awake() => health = GetComponent<Health>();
        private void OnEnable() => health.Died += ClearAll;
        private void OnDisable() => health.Died -= ClearAll;

        private void Update()
        {
            for (int i = bleeds.Count - 1; i >= 0; i--)
            {
                Bleed b = bleeds[i];
                if (Time.time >= b.endsAt || !health.IsAlive) { bleeds.RemoveAt(i); continue; }
                if (Time.time < b.nextTick) continue;
                b.nextTick += b.tickInterval;
                if (b.attacker != null) b.attacker.DealDamage(health, b.damagePerTick, Vector3.zero, b.source);
                else health.TakeDamage(b.damagePerTick, Vector3.zero, null, b.source);
            }
        }

        /// <summary>Aturde; devuelve la duración real tras rendimientos decrecientes (0 si inmune).</summary>
        public float ApplyStun(float duration)
        {
            float real = duration * DiminishingFactor(ref stunDr);
            if (real <= 0f) { Immune?.Invoke(); return 0f; }
            stunUntil = Mathf.Max(stunUntil, Time.time + real);
            Interrupted?.Invoke();
            return real;
        }

        /// <summary>Ralentiza un <paramref name="fraction"/> (0,4 = 40 % más lento). Se queda la más fuerte.</summary>
        public void ApplySlow(float fraction, float duration)
        {
            fraction = Mathf.Clamp01(fraction);
            if (Time.time < slowUntil && fraction < slowFraction) return;
            slowFraction = fraction;
            slowUntil = Time.time + duration;
        }

        /// <summary>Sangrado de <paramref name="attacker"/>: reemplaza al suyo anterior si lo había.</summary>
        public void ApplyBleed(Health attacker, float damagePerTick, float tickInterval, float duration, string source = "Sangrado")
        {
            Bleed existing = bleeds.Find(b => b.attacker == attacker);
            if (existing == null) { existing = new Bleed { attacker = attacker }; bleeds.Add(existing); }
            existing.source = source;
            existing.damagePerTick = damagePerTick;
            existing.tickInterval = Mathf.Max(tickInterval, 0.05f);
            existing.nextTick = Time.time + existing.tickInterval;
            existing.endsAt = Time.time + duration;
        }

        public void ApplyBuff(float damageMultiplier, float lifesteal, float duration)
        {
            buffDamageMultiplier = damageMultiplier;
            buffLifesteal = lifesteal;
            buffUntil = Time.time + duration;
        }

        /// <summary>Interrumpe lo que se esté cargando o anunciando, sin aturdir.</summary>
        public void Interrupt() => Interrupted?.Invoke();

        // ------------------------------------------------------------------

        private float DiminishingFactor(ref Diminishing dr)
        {
            if (Time.time >= dr.resetAt) dr.count = 0;
            dr.resetAt = Time.time + diminishingWindow;
            int stage = dr.count;
            dr.count = Mathf.Min(dr.count + 1, 3);
            switch (stage)
            {
                case 0: return 1f;
                case 1: return 0.5f;
                case 2: return 0.25f;
                default: return 0f;
            }
        }

        private int DiminishingStage(ref Diminishing dr)
        {
            return Time.time >= dr.resetAt ? 0 : dr.count;
        }

        private void ClearAll()
        {
            stunUntil = 0f;
            slowUntil = 0f;
            buffUntil = 0f;
            bleeds.Clear();
            stunDr = default;
        }
    }
}
