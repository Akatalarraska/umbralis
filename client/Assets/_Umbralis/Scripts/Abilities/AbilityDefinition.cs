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

        public AbilityContext(AbilityCaster caster, Vector3 origin, Vector3 direction, Vector3 point)
        {
            Caster = caster;
            Origin = origin;
            Direction = direction;
            Point = point;
        }
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
        [Min(0f)] public float cooldown = 1f;
        [Min(0.5f)] public float range = 5f;
        public AimMode aimMode = AimMode.Direction;

        [Tooltip("Segundos que el personaje se queda mirando hacia donde lanzó.")]
        [Min(0f)] public float faceLockDuration = 0.25f;

        [Header("Efectos")]
        [Tooltip("Material del efecto visual temporal (opcional).")]
        public Material fxMaterial;

        /// <summary>Radio que muestra el indicador en modo Point.</summary>
        public virtual float AimRadius => 1f;

        public abstract void Execute(in AbilityContext context);

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

        /// <summary>Aplica daño a todos los Health enemigos dentro de una esfera.</summary>
        protected static int DamageInSphere(Vector3 center, float radius, float damage, Team myTeam, Vector3 knockbackDirection, float coneDegrees = 360f, Vector3 coneForward = default)
        {
            int hits = 0;
            float halfCone = coneDegrees * 0.5f;
            // Copia porque TakeDamage puede disparar eventos que modifiquen la lista.
            foreach (Health h in Health.All.ToArray())
            {
                if (h.Team == myTeam || !h.IsAlive) continue;

                Vector3 to = h.transform.position - center;
                to.y = 0f;
                if (to.sqrMagnitude > radius * radius) continue;
                if (coneDegrees < 360f && Vector3.Angle(coneForward, to) > halfCone) continue;

                Vector3 push = knockbackDirection == Vector3.zero ? to.normalized : knockbackDirection;
                h.TakeDamage(damage, push);
                hits++;
            }
            return hits;
        }
    }
}
