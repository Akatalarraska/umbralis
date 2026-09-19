using UnityEngine;

namespace Umbralis.Abilities
{
    /// <summary>Golpe cuerpo a cuerpo: daña a todo enemigo dentro de un cono delante del jugador.</summary>
    [CreateAssetMenu(menuName = "Umbralis/Habilidades/Golpe (melé)", fileName = "Melee")]
    public sealed class MeleeAbility : AbilityDefinition
    {
        [Header("Golpe")]
        [Range(10f, 360f)] public float coneDegrees = 100f;

        public override void Execute(in AbilityContext context)
        {
            Vector3 feet = context.Caster.transform.position;
            DamageInSphere(feet, range, damage, context.Caster.Health, context.Direction, coneDegrees, context.Direction);

            // Tajo: una placa fina delante del personaje durante un instante.
            Vector3 pos = context.Origin + context.Direction * (range * 0.5f);
            SpawnFx(PrimitiveType.Cube, pos, Quaternion.LookRotation(context.Direction, Vector3.up),
                new Vector3(range * 0.9f, 0.08f, range), 0.1f);
        }
    }
}
