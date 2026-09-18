using UnityEngine;

namespace Umbralis.Core
{
    /// <summary>
    /// Ajustes globales que se aplican una sola vez al arrancar la escena:
    /// orientación horizontal bloqueada, tasa de refresco objetivo y evitar
    /// que la pantalla del móvil se apague mientras se juega.
    /// Colócalo en un GameObject vacío de la escena (el menú Umbralis lo hace solo).
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Tooltip("FPS objetivo en el dispositivo. 60 es lo habitual en móviles.")]
        [SerializeField] private int targetFrameRate = 60;

        private void Awake()
        {
            // Bloqueamos la orientación también desde código, por si los Player
            // Settings no se han aplicado en la compilación.
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            Application.targetFrameRate = targetFrameRate;

            // Un prototipo de combate se prueba durante minutos sin tocar botones
            // "reales"; sin esto Android apagaría la pantalla.
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
