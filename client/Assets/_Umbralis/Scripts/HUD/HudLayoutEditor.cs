using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Umbralis.HUD
{
    /// <summary>
    /// Modo de edición del HUD: un botón "HUD" lo activa; mientras está activo
    /// los botones de combate no lanzan nada y se pueden arrastrar; el panel
    /// de edición permite agrandar o encoger el seleccionado, restablecer todo
    /// y salir. La disposición se guarda en PlayerPrefs y se aplica al arrancar.
    /// </summary>
    public sealed class HudLayoutEditor : MonoBehaviour
    {
        [Header("Controles")]
        [SerializeField] private Button toggleButton;
        [SerializeField] private GameObject editPanel;
        [SerializeField] private Button smallerButton;
        [SerializeField] private Button biggerButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button doneButton;
        [SerializeField] private Text hintLabel;

        [Tooltip("Cuánto cambia la escala con cada pulsación de + o −.")]
        [SerializeField, Range(0.05f, 0.5f)] private float scaleStep = 0.1f;

        private const string PrefsKeyBase = "umbralis.hud.layout.v1";
        private static string profile = "default";
        private static string PrefsKey => PrefsKeyBase + "." + profile;

        /// <summary>
        /// Cambia el perfil de disposición (uno por especialización): vuelve a
        /// los valores por defecto y aplica lo guardado para ese perfil.
        /// </summary>
        public static void SetProfile(string name)
        {
            if (string.IsNullOrEmpty(name)) name = "default";
            if (name == profile && instance != null && instance.savedLoaded) return;
            profile = name;
            if (instance == null) return;
            instance.savedLoaded = false;
            foreach (HudEditableElement element in elements)
            {
                element.ResetToDefault();
                instance.ApplySaved(element);
            }
        }

        public static bool IsEditing { get; private set; }

        private static readonly List<HudEditableElement> elements = new List<HudEditableElement>();
        private static HudEditableElement selected;
        private static HudLayoutEditor instance;

        [Serializable]
        private struct ElementState
        {
            public string id;
            public float x, y, scale;
        }

        [Serializable]
        private struct LayoutState
        {
            public List<ElementState> elements;
        }

        // ------------------------------------------------------------------
        // Registro de elementos (lo hacen ellos mismos al activarse)
        // ------------------------------------------------------------------

        public static void Register(HudEditableElement element)
        {
            if (!elements.Contains(element)) elements.Add(element);
            if (instance != null) instance.ApplySaved(element);
        }

        public static void Unregister(HudEditableElement element)
        {
            elements.Remove(element);
            if (selected == element) selected = null;
        }

        public static void Select(HudEditableElement element)
        {
            if (selected != null) selected.Selected = false;
            selected = element;
            if (selected != null) selected.Selected = true;
            if (instance != null) instance.RefreshHint();
        }

        // ------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            IsEditing = false;
            if (editPanel != null) editPanel.SetActive(false);

            if (toggleButton != null) toggleButton.onClick.AddListener(() => SetEditing(!IsEditing));
            if (smallerButton != null) smallerButton.onClick.AddListener(() => ScaleSelected(-scaleStep));
            if (biggerButton != null) biggerButton.onClick.AddListener(() => ScaleSelected(scaleStep));
            if (resetButton != null) resetButton.onClick.AddListener(ResetAll);
            if (doneButton != null) doneButton.onClick.AddListener(() => SetEditing(false));
        }

        private void Start()
        {
            // Los elementos que se registraron antes de que existiera el editor.
            foreach (HudEditableElement element in elements) ApplySaved(element);
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
            IsEditing = false;
        }

        private void SetEditing(bool editing)
        {
            IsEditing = editing;
            if (editPanel != null) editPanel.SetActive(editing);
            if (!editing)
            {
                Select(null);
                Save();
            }
            RefreshHint();
        }

        private void ScaleSelected(float delta)
        {
            if (selected == null) return;
            selected.Scale += delta;
            RefreshHint();
        }

        private void ResetAll()
        {
            foreach (HudEditableElement element in elements) element.ResetToDefault();
            PlayerPrefs.DeleteKey(PrefsKey);
            RefreshHint();
        }

        private void RefreshHint()
        {
            if (hintLabel == null) return;
            hintLabel.text = selected == null
                ? "Toca un botón para seleccionarlo; arrástralo para moverlo"
                : $"{selected.Id}  ×{selected.Scale:0.0}";
        }

        // ------------------------------------------------------------------
        // Guardado
        // ------------------------------------------------------------------

        private LayoutState saved;
        private bool savedLoaded;

        private void Save()
        {
            var state = new LayoutState { elements = new List<ElementState>() };
            foreach (HudEditableElement element in elements)
            {
                state.elements.Add(new ElementState
                {
                    id = element.Id,
                    x = element.Position.x,
                    y = element.Position.y,
                    scale = element.Scale,
                });
            }
            PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(state));
            PlayerPrefs.Save();
            saved = state;
            savedLoaded = true;
        }

        private void ApplySaved(HudEditableElement element)
        {
            if (!savedLoaded)
            {
                savedLoaded = true;
                string json = PlayerPrefs.GetString(PrefsKey, string.Empty);
                saved = string.IsNullOrEmpty(json) ? default : JsonUtility.FromJson<LayoutState>(json);
            }
            if (saved.elements == null) return;

            foreach (ElementState s in saved.elements)
            {
                if (s.id != element.Id) continue;
                element.Position = new Vector2(s.x, s.y);
                element.Scale = s.scale;
                return;
            }
        }
    }
}
