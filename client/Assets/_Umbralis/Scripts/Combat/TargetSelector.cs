using System;
using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Objetivo seleccionado del jugador. Se elige tocando a un enemigo en la
    /// pantalla, con el botón de cambiar (pasa al siguiente por cercanía) o
    /// automáticamente al lanzar una habilidad sin objetivo. Se pierde si el
    /// objetivo muere o se aleja demasiado.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public sealed class TargetSelector : MonoBehaviour
    {
        [Tooltip("Distancia a partir de la cual se suelta el objetivo.")]
        [SerializeField, Min(1f)] private float maxDistance = 30f;

        [Tooltip("Capas contra las que se lanza el rayo del toque (los enemigos deben tener collider).")]
        [SerializeField] private LayerMask tapMask = ~0;

        public Health Current { get; private set; }
        public bool HasTarget => Current != null && Current.IsAlive;

        /// <summary>Se dispara al cambiar de objetivo (puede ser null).</summary>
        public event Action<Health> Changed;

        private Health self;

        private void Awake() => self = GetComponent<Health>();

        private void Update()
        {
            if (Current == null) return;
            if (!Current.IsAlive || (Current.transform.position - transform.position).sqrMagnitude > maxDistance * maxDistance)
                Select(null);
        }

        public void Select(Health target)
        {
            if (target == Current) return;
            Current = target;
            Changed?.Invoke(Current);
        }

        /// <summary>Toque en pantalla: si cae sobre un enemigo, lo selecciona.</summary>
        public bool TrySelectAtScreenPoint(Vector2 screenPosition)
        {
            Camera cam = Camera.main;
            if (cam == null) return false;

            Ray ray = cam.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 200f, tapMask, QueryTriggerInteraction.Ignore)) return false;

            Health health = hit.collider.GetComponentInParent<Health>();
            if (health == null || health == self || health.Team == self.Team || !health.IsAlive) return false;

            Select(health);
            return true;
        }

        /// <summary>Pasa al siguiente enemigo por cercanía (el más cercano si no hay ninguno).</summary>
        public void CycleNext()
        {
            Health best = null;
            float bestDistance = float.PositiveInfinity;
            float currentDistance = HasTarget ? Distance(Current) : -1f;

            // El más cercano entre los que están más lejos que el actual; si no
            // hay ninguno, empezamos otra vez por el más cercano de todos.
            Health nearest = null;
            float nearestDistance = float.PositiveInfinity;
            foreach (Health h in Health.All)
            {
                if (h == self || h.Team == self.Team || !h.IsAlive) continue;
                float d = Distance(h);
                if (d > maxDistance) continue;

                if (d < nearestDistance) { nearestDistance = d; nearest = h; }
                if (h != Current && d >= currentDistance && d < bestDistance) { bestDistance = d; best = h; }
            }

            Select(best != null ? best : nearest);
        }

        private float Distance(Health h) => Vector3.Distance(h.transform.position, transform.position);
    }
}
