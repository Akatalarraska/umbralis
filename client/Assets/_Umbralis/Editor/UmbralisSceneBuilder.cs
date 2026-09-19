using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Umbralis.Core;
using Umbralis.Player;
using Umbralis.TouchControls;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Umbralis.EditorTools
{
    /// <summary>
    /// Menú "Umbralis" del editor. Monta desde código todo lo que de otro modo
    /// habría que arrastrar a mano en la escena: suelo, jugador (cápsula),
    /// cámara, HUD con las dos zonas táctiles y el joystick, EventSystem y
    /// bootstrap. Se puede volver a ejecutar: sobrescribe la escena generada.
    /// </summary>
    public static class UmbralisSceneBuilder
    {
        private const string RootFolder = "Assets/_Umbralis";
        private const string ScenesFolder = RootFolder + "/Scenes";
        private const string MaterialsFolder = RootFolder + "/Materials";
        private const string ScenePath = ScenesFolder + "/CombatPrototype.unity";

        // Resolución de referencia del HUD. Los tamaños de UI (radio del joystick,
        // botones) se expresan en estas unidades y el CanvasScaler los adapta.
        private static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

        [MenuItem("Umbralis/1. Crear escena de prueba de combate")]
        public static void CreateCombatScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            BuildCombatScene();
        }

        /// <summary>Genera la escena sin preguntar nada (uso desde menú y desde batch mode).</summary>
        public static void BuildCombatScene()
        {
            EnsureFolder(ScenesFolder);
            EnsureFolder(MaterialsFolder);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateLight();
            CreateArena();
            GameObject player = CreatePlayer();
            GameObject camera = CreateCamera();
            CreateHud(out FloatingJoystick joystick, out CameraLookZone lookZone);
            CreateEventSystem();
            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();

            // Cableado de referencias entre componentes.
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            SetReference(movement, "joystick", joystick);
            SetReference(movement, "cameraTransform", camera.transform);

            ThirdPersonCamera orbit = camera.GetComponent<ThirdPersonCamera>();
            SetReference(orbit, "target", player.transform);
            SetReference(orbit, "lookZone", lookZone);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings();

            Debug.Log($"[Umbralis] Escena creada y guardada en {ScenePath}. Ya está en la lista de escenas de compilación.");
        }

        [MenuItem("Umbralis/2. Aplicar ajustes de Android")]
        public static void ApplyAndroidSettings()
        {
            PlayerSettings.productName = "Umbralis";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.umbralis.prototype");

            // Orientación horizontal fija. GameBootstrap la refuerza en runtime.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

            // IL2CPP + ARM64: los móviles recientes ya no ejecutan apps de 32 bits.
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.Log("[Umbralis] Cambiando la plataforma activa a Android (puede tardar un poco)...");
                if (Application.isBatchMode)
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                else
                    EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Android, BuildTarget.Android);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Umbralis] Ajustes de Android aplicados: nombre, identificador, orientación LandscapeLeft, IL2CPP/ARM64.");
        }

        [MenuItem("Umbralis/3. Compilar APK (Builds/Umbralis.apk)")]
        public static void BuildAndroidApk()
        {
            BuildAndroid(BuildOptions.None);
        }

        [MenuItem("Umbralis/4. Compilar e instalar en el móvil (Build And Run)")]
        public static void BuildAndRunAndroid()
        {
            BuildAndroid(BuildOptions.AutoRunPlayer);
        }

        private static void BuildAndroid(BuildOptions options)
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("[Umbralis] La plataforma activa no es Android. Ejecuta antes 'Umbralis → 2. Aplicar ajustes de Android'.");
                return;
            }

            const string output = "Builds/Umbralis.apk";
            System.IO.Directory.CreateDirectory("Builds");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = BuildTarget.Android,
                options = options,
            });

            var summary = report.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
                Debug.Log($"[Umbralis] APK generado en {output} ({summary.totalSize / (1024 * 1024)} MB).");
            else
                Debug.LogError($"[Umbralis] La compilación terminó con {summary.result}: {summary.totalErrors} errores.");
        }

        // ------------------------------------------------------------------
        // Puntos de entrada para línea de comandos (-batchmode -executeMethod)
        // ------------------------------------------------------------------

        /// <summary>Escena + ajustes Android en una sola pasada. Lanzar con -buildTarget Android.</summary>
        public static void BatchSetup()
        {
            BuildCombatScene();
            ApplyAndroidSettings();
        }

        /// <summary>Escena + ajustes + APK. Lanzar con -buildTarget Android.</summary>
        public static void BatchSetupAndBuild()
        {
            BatchSetup();
            BuildAndroidApk();
        }

        // ------------------------------------------------------------------
        // Escena 3D
        // ------------------------------------------------------------------

        private static void CreateLight()
        {
            var go = new GameObject("Directional Light");
            Light light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void CreateArena()
        {
            Material ground = CreateMaterial("Ground", new Color(0.30f, 0.32f, 0.35f));
            Material obstacle = CreateMaterial("Obstacle", new Color(0.55f, 0.45f, 0.35f));

            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Ground";
            plane.transform.localScale = new Vector3(4f, 1f, 4f); // 40 x 40 m
            plane.GetComponent<Renderer>().sharedMaterial = ground;
            plane.isStatic = true;

            // Unos cubos repartidos para tener referencias visuales al moverse y
            // comprobar que la cámara no atraviesa obstáculos.
            var parent = new GameObject("Obstacles");
            Vector3[] positions =
            {
                new Vector3(6f, 1f, 6f), new Vector3(-7f, 1f, 4f), new Vector3(5f, 1f, -8f),
                new Vector3(-6f, 1f, -6f), new Vector3(0f, 1f, 12f), new Vector3(12f, 1f, 0f),
            };
            foreach (Vector3 position in positions)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Obstacle";
                cube.transform.SetParent(parent.transform, false);
                cube.transform.position = position;
                cube.transform.localScale = new Vector3(2f, 2f, 2f);
                cube.GetComponent<Renderer>().sharedMaterial = obstacle;
                cube.isStatic = true;
            }
        }

        private static GameObject CreatePlayer()
        {
            Material playerMat = CreateMaterial("Player", new Color(0.25f, 0.55f, 0.95f));
            Material markerMat = CreateMaterial("PlayerMarker", new Color(0.95f, 0.85f, 0.2f));

            // Raíz en los pies del personaje: es lo más cómodo para posicionarlo.
            var root = new GameObject("Player");
            CharacterController controller = root.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0f, 1f, 0f);
            controller.slopeLimit = 45f;
            controller.stepOffset = 0.3f;
            root.AddComponent<PlayerMovement>();

            // Cuerpo: cápsula sin collider (el CharacterController ya hace de collider).
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            body.GetComponent<Renderer>().sharedMaterial = playerMat;

            // Cubito delante para ver hacia dónde mira el personaje.
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "FacingMarker";
            marker.transform.SetParent(root.transform, false);
            marker.transform.localPosition = new Vector3(0f, 1.2f, 0.55f);
            marker.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Object.DestroyImmediate(marker.GetComponent<Collider>());
            marker.GetComponent<Renderer>().sharedMaterial = markerMat;

            return root;
        }

        private static GameObject CreateCamera()
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            Camera cam = go.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 200f;
            cam.fieldOfView = 60f;
            go.AddComponent<AudioListener>();
            go.AddComponent<ThirdPersonCamera>();
            go.transform.position = new Vector3(0f, 3f, -6f);
            return go;
        }

        // ------------------------------------------------------------------
        // HUD
        // ------------------------------------------------------------------

        private static void CreateHud(out FloatingJoystick joystick, out CameraLookZone lookZone)
        {
            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            // Sprite circular incluido en Unity; nos ahorra crear texturas.
            Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

            // Mitad izquierda: joystick flotante.
            GameObject leftZone = CreateZone("JoystickZone", canvasGo.transform, new Vector2(0f, 0f), new Vector2(0.5f, 1f));
            joystick = leftZone.AddComponent<FloatingJoystick>();
            Image background = CreateImage("JoystickBackground", leftZone.transform, knob, 260f, new Color(1f, 1f, 1f, 0.35f));
            Image handle = CreateImage("JoystickHandle", background.transform, knob, 110f, new Color(1f, 1f, 1f, 0.8f));
            SetReference(joystick, "background", background.rectTransform);
            SetReference(joystick, "handle", handle.rectTransform);

            // Mitad derecha: giro de cámara.
            GameObject rightZone = CreateZone("LookZone", canvasGo.transform, new Vector2(0.5f, 0f), new Vector2(1f, 1f));
            lookZone = rightZone.AddComponent<CameraLookZone>();
        }

        /// <summary>Panel invisible que recibe toques en la porción de pantalla indicada.</summary>
        private static GameObject CreateZone(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // Color totalmente transparente pero con Raycast Target activo:
            // el EventSystem sigue detectando los toques.
            var image = go.GetComponent<Image>();
            image.color = Color.clear;
            image.raycastTarget = true;
            return go;
        }

        private static Image CreateImage(string name, Transform parent, Sprite sprite, float size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(size, size);
            rt.anchoredPosition = Vector2.zero;

            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false; // los toques los gestiona la zona padre
            return image;
        }

        private static void CreateEventSystem()
        {
            var go = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            // Proyecto con Input System (nuevo): módulo compatible.
            go.AddComponent<InputSystemUIInputModule>();
#else
            // Proyecto con Input Manager clásico.
            go.AddComponent<StandaloneInputModule>();
#endif
        }

        // ------------------------------------------------------------------
        // Utilidades
        // ------------------------------------------------------------------

        private static Material CreateMaterial(string name, Color color)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                existing.color = color;
                EditorUtility.SetDirty(existing);
                return existing;
            }

            // URP Lit si el proyecto es URP; Standard como respaldo.
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            var material = new Material(shader) { color = color };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        /// <summary>Asigna un campo [SerializeField] privado sin exponerlo como público.</summary>
        private static void SetReference(Object target, string fieldName, Object value)
        {
            var serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogError($"[Umbralis] No existe el campo '{fieldName}' en {target.GetType().Name}.");
                return;
            }
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Deja CombatPrototype como única escena de compilación. Si solo se
        /// añadiera al final, la SampleScene de la plantilla (índice 0) sería la
        /// que arrancase en el móvil.
        /// </summary>
        private static void AddSceneToBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.RemoveAll(s => s.path == ScenePath);
            scenes.ForEach(s => s.enabled = false);
            scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        /// <summary>Crea la carpeta (y sus padres) dentro de Assets si no existe.</summary>
        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            string parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
