using UnityEngine;

namespace Umbralis.Enemies
{
    using Combat;

    /// <summary>
    /// IA básica de enemigo cuerpo a cuerpo: espera hasta que el jugador se
    /// acerca, lo persigue, y al tenerlo a tiro anuncia el golpe con una zona
    /// en el suelo durante <see cref="telegraphDuration"/> segundos antes de
    /// pegar. La zona se fija al empezar el aviso, así que se puede esquivar
    /// saliendo de ella (o con la Embestida).
    /// </summary>
    [RequireComponent(typeof(Health), typeof(CharacterController), typeof(HitReaction))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        private enum State { Idle, Chase, Telegraph, Recover }

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

        private Health health;
        private CharacterController controller;
        private HitReaction hitReaction;
        private State state = State.Idle;
        private Health target;
        private float stateTimer;
        private Vector3 hitCenter;

        private void Awake()
        {
            health = GetComponent<Health>();
            controller = GetComponent<CharacterController>();
            hitReaction = GetComponent<HitReaction>();
        }

        private void OnDisable()
        {
            // Al morir (Respawner nos apaga) el aviso no debe quedarse en el suelo.
            if (telegraph != null) telegraph.Hide();
            state = State.Idle;
        }

        private void Update()
        {
            if (!controller.enabled || !health.IsAlive) return;

            Vector3 move = hitReaction.ConsumeKnockback(Time.deltaTime);

            switch (state)
            {
                case State.Idle:
                    target = Health.FindNearestHostile(health.Team, transform.position, aggroRange);
                    if (target != null) state = State.Chase;
                    break;

                case State.Chase:
                    if (!TargetValid()) { state = State.Idle; break; }
                    Vector3 to = ToTarget();
                    Face(to);
                    if (to.magnitude <= attackRange)
                        StartTelegraph(to);
                    else if (!hitReaction.IsKnockedBack)
                        move += to.normalized * (moveSpeed * Time.deltaTime);
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
                h.TakeDamage(damage, transform.forward, health);
            }

            stateTimer = recoverDuration;
            state = State.Recover;
        }
    }
}
