using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Muñeco de entrenamiento: no ataca ni se mueve, solo se queda pegado al
    /// suelo y encaja los empujones de <see cref="HitReaction"/>. El parpadeo
    /// y la reaparición los ponen HitReaction y Respawner en el mismo objeto.
    /// </summary>
    [RequireComponent(typeof(CharacterController), typeof(HitReaction))]
    public sealed class TrainingDummy : MonoBehaviour
    {
        private CharacterController controller;
        private HitReaction hitReaction;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            hitReaction = GetComponent<HitReaction>();
        }

        private void Update()
        {
            if (!controller.enabled) return;
            // Gravedad mínima para que se quede pegado al suelo tras un empujón.
            controller.Move(hitReaction.ConsumeKnockback(Time.deltaTime) + Vector3.down * (5f * Time.deltaTime));
        }
    }
}
