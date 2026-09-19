using System.Collections.Generic;
using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Passives
{
    /// <summary>
    /// Brasas del juicio / Almas errantes (pasiva del Inquisidor / Segador):
    /// algunos golpes dejan una brasa en el suelo bajo la víctima; al pisarla,
    /// el dueño se cura y genera recurso. Las brasas son discos planos que
    /// desaparecen solas pasado un tiempo.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public sealed class EmberPassive : MonoBehaviour
    {
        public float chance = 0.3f;
        public float healFraction = 0.05f;
        public float resourceGain = 10f;
        public float lifetime = 10f;
        public float pickupRadius = 0.9f;
        public Material material;

        private Health health;
        private ClassResource resource;
        private readonly List<(Transform t, float dies)> embers = new List<(Transform, float)>();

        private void Awake()
        {
            health = GetComponent<Health>();
            resource = GetComponent<ClassResource>();
        }

        private void OnEnable() => Health.AnyDamaged += OnAnyDamaged;

        private void OnDisable()
        {
            Health.AnyDamaged -= OnAnyDamaged;
            foreach (var e in embers) if (e.t != null) Destroy(e.t.gameObject);
            embers.Clear();
        }

        private void Update()
        {
            for (int i = embers.Count - 1; i >= 0; i--)
            {
                (Transform t, float dies) = embers[i];
                if (t == null || Time.time >= dies) { if (t != null) Destroy(t.gameObject); embers.RemoveAt(i); continue; }

                Vector3 to = t.position - transform.position;
                to.y = 0f;
                if (to.sqrMagnitude > pickupRadius * pickupRadius) continue;

                health.Heal(health.Max * healFraction);
                if (resource != null) resource.Gain(resourceGain);
                Destroy(t.gameObject);
                embers.RemoveAt(i);
            }
        }

        private void OnAnyDamaged(Health attacker, Health victim, float amount, string source)
        {
            if (attacker != health || victim == null || Random.value > chance) return;

            GameObject ember = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ember.name = "Brasa";
            Destroy(ember.GetComponent<Collider>());
            Vector3 at = victim.transform.position;
            at.y = 0.03f;
            ember.transform.position = at;
            ember.transform.localScale = new Vector3(0.6f, 0.02f, 0.6f);
            if (material != null) ember.GetComponent<Renderer>().sharedMaterial = material;
            embers.Add((ember.transform, Time.time + lifetime));
        }
    }
}
