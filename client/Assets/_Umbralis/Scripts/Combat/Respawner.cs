using System.Collections;
using UnityEngine;

namespace Umbralis.Combat
{
    /// <summary>
    /// Al morir, oculta el cuerpo y apaga los componentes indicados; pasado un
    /// tiempo vuelve a su sitio de origen con toda la vida. Sirve para muñecos,
    /// enemigos y el propio jugador en el prototipo (no hay pantalla de muerte).
    /// </summary>
    [RequireComponent(typeof(Health))]
    public sealed class Respawner : MonoBehaviour
    {
        [SerializeField, Min(0.5f)] private float delay = 3f;
        [Tooltip("Renderers que se ocultan mientras está muerto.")]
        [SerializeField] private Renderer[] renderers;
        [Tooltip("Componentes que se desactivan mientras está muerto (movimiento, IA, habilidades...).")]
        [SerializeField] private Behaviour[] behaviours;

        private Health health;
        private CharacterController controller;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;

        private void Awake()
        {
            health = GetComponent<Health>();
            controller = GetComponent<CharacterController>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        private void OnDied() => StartCoroutine(Respawn());

        private IEnumerator Respawn()
        {
            SetAlive(false);
            yield return new WaitForSeconds(delay);

            // Con el CharacterController apagado se puede teletransportar sin pelear con él.
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            SetAlive(true);
            health.Revive();
        }

        private void SetAlive(bool alive)
        {
            if (controller != null) controller.enabled = alive;
            foreach (Renderer r in renderers) if (r != null) r.enabled = alive;
            foreach (Behaviour b in behaviours) if (b != null) b.enabled = alive;
        }
    }
}
