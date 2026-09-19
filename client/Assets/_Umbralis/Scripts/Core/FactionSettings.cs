using System;
using UnityEngine;

namespace Umbralis.Core
{
    public enum Faction { LlamaBlanca, PactoOscuro }

    /// <summary>
    /// Facción del jugador. Solo cambia lo visual: las clases espejo
    /// (Templario/Juramentado) comparten assets y números, y cada asset lleva
    /// su nombre, color y efecto para cada facción. Se guarda en PlayerPrefs.
    /// </summary>
    public static class FactionSettings
    {
        private const string PrefsKey = "umbralis.faction";
        private static Faction? current;

        public static Faction Current
        {
            get
            {
                if (!current.HasValue) current = (Faction)PlayerPrefs.GetInt(PrefsKey, (int)Faction.LlamaBlanca);
                return current.Value;
            }
            set
            {
                if (current.HasValue && current.Value == value) return;
                current = value;
                PlayerPrefs.SetInt(PrefsKey, (int)value);
                PlayerPrefs.Save();
                Changed?.Invoke(value);
            }
        }

        public static bool IsPacto => Current == Faction.PactoOscuro;

        public static event Action<Faction> Changed;

        public static string Name(Faction f) => f == Faction.PactoOscuro ? "El Pacto Oscuro" : "La Llama Blanca";
    }
}
