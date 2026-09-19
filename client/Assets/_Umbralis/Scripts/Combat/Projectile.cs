using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Proyectil recto sin físicas: cada frame avanza y lanza un SphereCast sobre
    /// el tramo recorrido, así nunca atraviesa objetivos por ir rápido. Daña al
    /// primer Health de otro equipo y se destruye contra cualquier obstáculo.
    /// </summary>
    public sealed class Projectile : MonoBehaviour
    {
        private Health owner;
        private Vector3 direction;
        private float speed;
        private float damage;
        private float remainingDistance;
        private float radius;
        private string source;

        public void Launch(Health owner, Vector3 direction, float speed, float range, float damage, float radius, string source = null)
        {
            this.owner = owner;
            this.source = source;
            this.direction = direction.normalized;
            this.speed = speed;
            this.damage = damage;
            this.radius = radius;
            remainingDistance = range;
            transform.rotation = Quaternion.LookRotation(this.direction, Vector3.up);
        }

        private void Update()
        {
            float step = Mathf.Min(speed * Time.deltaTime, remainingDistance);
            remainingDistance -= step;

            if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, step, ~0, QueryTriggerInteraction.Ignore)
                && (owner == null || !hit.transform.IsChildOf(owner.transform)))
            {
                Health health = hit.collider.GetComponentInParent<Health>();
                if (health != null && owner == null) health.TakeDamage(damage, direction, null, source);
                else if (health != null && health.Team != owner.Team) owner.DealDamage(health, damage, direction, source);

                Destroy(gameObject);
                return;
            }

            transform.position += direction * step;
            if (remainingDistance <= 0f) Destroy(gameObject);
        }
    }
}
