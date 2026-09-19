using System.Collections.Generic;
using UnityEngine;

namespace Umbralis.Enemies
{
    using Combat;

    /// <summary>
    /// IA básica de enemigo cuerpo a cuerpo: espera hasta que alguien hostil se
    /// acerca, lo persigue, y al tenerlo a tiro anuncia el golpe con una zona
    /// en el suelo durante <see cref="telegraphDuration"/> segundos antes de
    /// pegar. La zona se fija al empezar el aviso, así que se puede esquivar
    /// saliendo de ella (o con la Embestida).
    ///
    /// A quién ataca lo decide la <b>amenaza</b>: cada punto de daño recibido
    /// suma amenaza a quien lo hizo, las habilidades de tanque suman extra, y
    /// una provocación fuerza el objetivo durante unos segundos. Sin amenaza,
    /// el hostil más cercano.
    /// </summary>
    [RequireComponent(typeof(Health), typeof(CharacterController), typeof(HitReaction))]
    [RequireComponent(typeof(StatusEffects))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        private enum State { Idle, Chase, Telegraph, Recover }

        /// <summary>True mientras anuncia un golpe: es cuando se le puede interrumpir.</summary>
        public bool IsTelegraphing => state == State.Telegraph;

        [Header("Referencias")]
        [SerializeField] private AttackTelegraph telegraph;

        [Header("Movimiento")]
        [SerializeField, Min(0f)] private float moveSpeed = 3.5f;
        [SerializeField, Min(0f)] private float turnSpeed = 540f;
        [Tooltip("Distancia a la que se fija en el jugador.")]
        [SerializeField, Min(1f)] private float aggroRange = 12f;
        [Tooltip("Distancia a la que deja de perseguir y ataca.")]
        [SerializeField, Min(0.5f)] private float attackRange = 2.2f;

        [Header("Ataque")]
        [SerializeField, Min(0f)] private float damage = 15f;
        [Tooltip("Segundos de aviso antes del impacto. El diseño exige al menos 1,5.")]
        [SerializeField, Min(0.5f)] private float telegraphDuration = 1.5f;
        [Tooltip("Radio de la zona de impacto en el suelo.")]
        [SerializeField, Min(0.5f)] private float hitRadius = 2f;
        [Tooltip("Distancia del centro de la zona por delante del enemigo.")]
        [SerializeField, Min(0f)] private float hitOffset = 1.2f;
        [Tooltip("Segundos tras el golpe hasta poder volver a atacar.")]
        [SerializeField, Min(0f)] private float recoverDuration = 1.2f;

        [Header("Amenaza")]
        [Tooltip("Cuánto más amenaza necesita otro para robar el objetivo actual (1,2 = un 20 % más).")]
        [SerializeField, Min(1f)] private float switchThreshold = 1.2f;

        /// <summary>Quién tiene el objetivo ahora (para depurar y para el HUD).</summary>
        public Health CurrentTarget => target;

        private readonly Dictionary<Health, float> threat = new Dictionary<Health, float>();
        private Health tauntedBy;
        private float tauntUntil;

        private Health health;
        private CharacterController controller;
        private HitReaction hitReaction;
        private StatusEffects status;
        private State state = State.Idle;
        private Health target;
        private float stateTimer;
        private Vector3 hitCenter;

        private void Awake()
        {
            health = GetComponent<Health>();
            controller = GetComponent<CharacterController>();
            hitReaction = GetComponent<HitReaction>();
            status = GetComponent<StatusEffects>();
        }

        private void OnEnable()
        {
            status.Interrupted += OnInterrupted;
            Health.AnyDamaged += OnAnyDamaged;
        }

        private void OnDisable()
        {
            status.Interrupted -= OnInterrupted;
            Health.AnyDamaged -= OnAnyDamaged;
            threat.Clear();
            tauntedBy = null;
            // Al morir (Respawner nos apaga) el aviso no debe quedarse en el suelo.
            if (telegraph != null) telegraph.Hide();
            state = State.Idle;
        }

        /// <summary>El daño recibido se convierte en amenaza hacia quien lo hizo.</summary>
        private void OnAnyDamaged(Health attacker, Health victim, float amount, string source)
        {
            if (victim != health || attacker == null || attacker.Team == health.Team) return;
            AddThreat(attacker, amount);
        }

        /// <summary>Suma amenaza (p. ej. Tajo de barrido del Baluarte). Despierta al enemigo si estaba quieto.</summary>
        public void AddThreat(Health source, float amount)
        {
            if (source == null || amount <= 0f) return;
            threat.TryGetValue(source, out float current);
            threat[source] = current + amount;
            if (state == State.Idle) { target = source; state = State.Chase; }
        }

        /// <summary>Obliga a atacar a <paramref name="by"/> durante <paramref name="duration"/> s, y le da la amenaza más alta.</summary>
        public void Taunt(Health by, float duration)
        {
            if (by == null) return;
            tauntedBy = by;
            tauntUntil = Time.time + duration;
            float top = 0f;
            foreach (float t in threat.Values) top = Mathf.Max(top, t);
            threat[by] = top * switchThreshold + 10f; // al acabar la provocación sigue siendo el primero
            if (state == State.Idle || state == State.Chase) { target = by; state = State.Chase; }
        }

        /// <summary>Elige objetivo: provocador, si no el de más amenaza (con margen), si no el más cercano.</summary>
        private Health PickTarget()
        {
            if (tauntedBy != null && Time.time < tauntUntil && tauntedBy.IsAlive && InLeash(tauntedBy)) return tauntedBy;

            Health best = null;
            float bestThreat = 0f;
            float currentThreat = target != null && threat.TryGetValue(target, out float ct) ? ct : 0f;
            foreach (KeyValuePair<Health, float> pair in threat)
            {
                Health h = pair.Key;
                if (h == null || !h.IsAlive || !InLeash(h)) continue;
                if (pair.Value > bestThreat) { bestThreat = pair.Value; best = h; }
            }
            // El actual conserva el objetivo salvo que otro lo supere con margen.
            if (target != null && target.IsAlive && InLeash(target) && best != target && bestThreat < currentThreat * switchThreshold) return target;
            if (best != null) return best;

            return Health.FindNearestHostile(health.Team, transform.position, aggroRange);
        }

        private bool InLeash(Health h) => (h.transform.position - transform.position).sqrMagnitude <= aggroRange * aggroRange * 1.5f;

        /// <summary>Patada del jugador o aturdimiento: el golpe anunciado se cancela y pierde el turno.</summary>
        private void OnInterrupted()
        {
            if (state != State.Telegraph) return;
            if (telegraph != null) telegraph.Hide();
            stateTimer = recoverDuration;
            state = State.Recover;
        }

        private void Update()
        {
            if (!controller.enabled || !health.IsAlive) return;

            Vector3 move = hitReaction.ConsumeKnockback(Time.deltaTime);

            // Aturdido: ni se mueve ni piensa; solo encaja empujones y gravedad.
            if (status.IsStunned)
            {
                controller.Move(move + Vector3.down * (5f * Time.deltaTime));
                return;
            }

            switch (state)
            {
                case State.Idle:
                    threat.Clear(); // fuera de combate se olvida todo
                    target = PickTarget();
                    if (target != null) state = State.Chase;
                    break;

                case State.Chase:
                    target = PickTarget();
                    if (!TargetValid()) { state = State.Idle; break; }
                    Vector3 to = ToTarget();
                    Face(to);
                    if (to.magnitude <= attackRange)
                        StartTelegraph(to);
                    else if (!hitReaction.IsKnockedBack)
                        move += to.normalized * (moveSpeed * status.SpeedMultiplier * Time.deltaTime);
                    break;

                case State.Telegraph:
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f) Strike();
                    break;

                case State.Recover:
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f) state = State.Idle;
                    break;
            }

            // Gravedad mínima para quedarse pegado al suelo.
            controller.Move(move + Vector3.down * (5f * Time.deltaTime));
        }

        private bool TargetValid()
        {
            return target != null && target.IsAlive
                && (target.transform.position - transform.position).sqrMagnitude <= aggroRange * aggroRange * 1.5f;
        }

        private Vector3 ToTarget()
        {
            Vector3 to = target.transform.position - transform.position;
            to.y = 0f;
            return to;
        }

        private void Face(Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return;
            Quaternion wanted = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, wanted, turnSpeed * Time.deltaTime);
        }

        private void StartTelegraph(Vector3 toTarget)
        {
            // La zona se decide ahora y ya no se mueve: es lo que la hace esquivable.
            transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            hitCenter = transform.position + transform.forward * hitOffset;
            if (telegraph != null) telegraph.Show(hitCenter, hitRadius, telegraphDuration);
            stateTimer = telegraphDuration;
            state = State.Telegraph;
        }

        private void Strike()
        {
            if (telegraph != null) telegraph.Hide();

            foreach (Health h in Health.All.ToArray())
            {
                if (h.Team == health.Team || !h.IsAlive) continue;
                Vector3 to = h.transform.position - hitCenter;
                to.y = 0f;
                if (to.sqrMagnitude > hitRadius * hitRadius) continue;
                health.DealDamage(h, damage, transform.forward, "Golpe de Acechador");
            }

            stateTimer = recoverDuration;
            state = State.Recover;
        }
    }
}
