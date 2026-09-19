using System.Collections;
using UnityEngine;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Salto: el personaje salta en arco hasta el punto elegido (dentro del
    /// alcance) y al caer daña a los enemigos alrededor. Un toque rápido salta
    /// sobre el objetivo.
    /// </summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Salto", fileName = "Leap")]
    public sealed class LeapAbility : AbilityDefinition
    {
        [Header("Salto")]
        [Min(0.1f)] public float duration = 0.45f;
        [Min(0f)] public float height = 2.5f;
        [Tooltip("Radio del daño al caer.")]
        [Min(0.25f)] public float radius = 2.5f;

        public override float AimRadius => radius;
        private void OnValidate() => aimMode = AimMode.Point;

        public override void Execute(in AbilityContext context)
        {
            Vector3 destination = context.Point;
            destination.y = context.Caster.transform.position.y;
            // Sobre un objetivo cae justo delante de él, no encima.
            if (context.Target != null)
            {
                Vector3 back = (context.Caster.transform.position - context.Target.transform.position);
                back.y = 0f;
                destination = context.Target.transform.position + back.normalized * 1f;
            }

            context.Caster.Movement.Leap(destination, duration, height);
            context.Caster.StartCoroutine(LandAfter(context.Caster, destination));
        }

        private IEnumerator LandAfter(AbilityCaster caster, Vector3 destination)
        {
            yield return new WaitForSeconds(duration);
            if (!caster.isActiveAndEnabled) yield break;
            Vector3 center = caster.transform.position;
            DamageInSphere(center, radius, damage, caster.Health, Vector3.zero);
            SpawnFx(PrimitiveType.Cylinder, center + Vector3.up * 0.05f, Quaternion.identity, new Vector3(radius * 2f, 0.03f, radius * 2f), 0.15f);
        }
    }
}
