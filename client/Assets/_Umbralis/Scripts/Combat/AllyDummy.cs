using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Aliado de entrenamiento: no se mueve, pero pincha a los enemigos que
    /// tenga cerca cada cierto tiempo. Genera amenaza sobre sí mismo, así que
    /// los enemigos acaban yendo a por él: es lo que el tanque tiene que
    /// impedir con provocaciones e Interceptar. Sustituye a un jugador aliado
    /// hasta la fase de red.
    /// </summary>
    [RequireComponent(typeof(Health), typeof(CharacterController), typeof(HitReaction))]
    public sealed class AllyDummy : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float damage = 6f;
        [SerializeField, Min(0.1f)] private float interval = 1f;
        [SerializeField, Min(0.5f)] private float reach = 4f;

        private Health health;
        private CharacterController controller;
        private HitReaction hitReaction;
        private float nextHit;

        private void Awake()
        {
            health = GetComponent<Health>();
            controller = GetComponent<CharacterController>();
            hitReaction = GetComponent<HitReaction>();
        }

        private void Update()
        {
            if (!controller.enabled || !health.IsAlive) return;
            controller.Move(hitReaction.ConsumeKnockback(Time.deltaTime) + Vector3.down * (5f * Time.deltaTime));

            if (Time.time < nextHit) return;
            Health enemy = Health.FindNearestHostile(health.Team, transform.position, reach);
            if (enemy == null) return;
            nextHit = Time.time + interval;
            Vector3 to = enemy.transform.position - transform.position;
            to.y = 0f;
            transform.rotation = Quaternion.LookRotation(to.normalized, Vector3.up);
            health.DealDamage(enemy, damage, to.normalized, "Aliado");
        }
    }
}
