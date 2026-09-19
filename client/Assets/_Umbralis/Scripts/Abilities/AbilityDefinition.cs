using UnityEngine;
using Umbralis.Combat;

namespace Umbralis.Abilities
{
    /// <summary>Cómo se apunta la habilidad al mantener pulsado el botón.</summary>
    public enum AimMode
    {
        /// <summary>Se elige una dirección desde el jugador (franja en el suelo).</summary>
        Direction,
        /// <summary>Se elige un punto en el suelo dentro del alcance (disco).</summary>
        Point,
    }

    /// <summary>Todo lo que una habilidad necesita saber en el momento de lanzarse.</summary>
    public readonly struct AbilityContext
    {
        public readonly AbilityCaster Caster;
        public readonly Vector3 Origin;      // centro del lanzador (altura del pecho)
        public readonly Vector3 Direction;   // horizontal, normalizada
        public readonly Vector3 Point;       // punto apuntado en el suelo
        public readonly Health Target;       // objetivo dirigido (puede ser null)

        public AbilityContext(AbilityCaster caster, Vector3 origin, Vector3 direction, Vector3 point, Health target = null)
        {
            Caster = caster;
            Origin = origin;
            Direction = direction;
            Point = point;
            Target = target;
        }

        /// <summary>Efectos de estado del objetivo, o null si no tiene (los muñecos no).</summary>
        public StatusEffects TargetStatus => Target != null ? Target.GetComponent<StatusEffects>() : null;
    }

    /// <summary>
    /// Datos y comportamiento de una habilidad. Cada tipo concreto (melé,
    /// proyectil, embestida, estallido) hereda y sobreescribe <see cref="Execute"/>.
    /// Los valores se ajustan en el Inspector del asset dentro de Data/.
    /// </summary>
    public abstract class AbilityDefinition : ScriptableObject
    {
        [Header("Presentación")]
        public string displayName = "Habilidad";
        public Color buttonColor = Color.white;

        [Header("Lanzamiento")]
        [Tooltip("Daño base. Cada tipo decide cómo lo aplica (cono, proyectil, área...).")]
        [Min(0f)] public float damage = 20f;
        [Min(0f)] public float cooldown = 1f;
        [Min(0.5f)] public float range = 5f;
        public AimMode aimMode = AimMode.Direction;

        [Header("Recurso")]
        [Tooltip("Recurso que consume al lanzarse. Si no hay suficiente, no se lanza.")]
        [Min(0f)] public float resourceCost = 0f;
        [Tooltip("Recurso que genera al lanzarse (las habilidades básicas generan, las fuertes gastan).")]
        [Min(0f)] public float resourceGain = 0f;

        [Tooltip("Segundos que el personaje se queda mirando hacia donde lanzó.")]
        [Min(0f)] public float faceLockDuration = 0.25f;

        [Header("Efectos")]
        [Tooltip("Material del efecto visual temporal (opcional).")]
        public Material fxMaterial;

        /// <summary>Radio que muestra el indicador en modo Point.</summary>
        public virtual float AimRadius => 1f;

        /// <summary>Si devuelve false la habilidad no se lanza: sin coste ni recarga.</summary>
        public virtual bool CanExecute(in AbilityContext context) => true;

        public abstract void Execute(in AbilityContext context);

        /// <summary>
        /// Aplica daño a los enemigos de un cono/esfera y además ejecuta
        /// <paramref name="onHit"/> sobre cada uno (para sangrados, aturdimientos...).
        /// </summary>
        protected static int DamageInSphere(Vector3 center, float radius, float damage, Health attacker, Vector3 knockbackDirection,
            float coneDegrees, Vector3 coneForward, System.Action<Health> onHit)
        {
            int hits = 0;
            float halfCone = coneDegrees * 0.5f;
            foreach (Health h in Health.All.ToArray())
            {
                if (h.Team == attacker.Team || !h.IsAlive) continue;
                Vector3 to = h.transform.position - center;
                to.y = 0f;
                if (to.sqrMagnitude > radius * radius) continue;
                if (coneDegrees < 360f && Vector3.Angle(coneForward, to) > halfCone) continue;

                Vector3 push = knockbackDirection == Vector3.zero ? to.normalized : knockbackDirection;
                attacker.DealDamage(h, damage, push);
                onHit?.Invoke(h);
                hits++;
            }
            return hits;
        }

        // ------------------------------------------------------------------
        // Utilidades comunes
        // ------------------------------------------------------------------

        /// <summary>Crea un primitivo sin collider que se autodestruye: efecto visual barato.</summary>
        protected GameObject SpawnFx(PrimitiveType type, Vector3 position, Quaternion rotation, Vector3 scale, float lifetime)
        {
            GameObject fx = GameObject.CreatePrimitive(type);
            fx.name = $"{displayName} FX";
            Object.Destroy(fx.GetComponent<Collider>());
            fx.transform.SetPositionAndRotation(position, rotation);
            fx.transform.localScale = scale;
            if (fxMaterial != null) fx.GetComponent<Renderer>().sharedMaterial = fxMaterial;
            Object.Destroy(fx, lifetime);
            return fx;
        }

        /// <summary>Aplica daño, en nombre de <paramref name="attacker"/>, a todos los Health enemigos dentro de una esfera.</summary>
        protected static int DamageInSphere(Vector3 center, float radius, float damage, Health attacker, Vector3 knockbackDirection, float coneDegrees = 360f, Vector3 coneForward = default)
        {
            int hits = 0;
            float halfCone = coneDegrees * 0.5f;
            // Copia porque TakeDamage puede disparar eventos que modifiquen la lista.
            foreach (Health h in Health.All.ToArray())
            {
                if (h.Team == attacker.Team || !h.IsAlive) continue;

                Vector3 to = h.transform.position - center;
                to.y = 0f;
                if (to.sqrMagnitude > radius * radius) continue;
                if (coneDegrees < 360f && Vector3.Angle(coneForward, to) > halfCone) continue;

                Vector3 push = knockbackDirection == Vector3.zero ? to.normalized : knockbackDirection;
                attacker.DealDamage(h, damage, push);
                hits++;
            }
            return hits;
        }
    }
}
