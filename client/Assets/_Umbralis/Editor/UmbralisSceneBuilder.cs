using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Umbralis.Abilities;
using Umbralis.Combat;
using Umbralis.Core;
using Umbralis.Enemies;
using Umbralis.HUD;
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
        private const string DataFolder = RootFolder + "/Data";
        private const string DevastadorFolder = DataFolder + "/Devastador";
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
            EnsureFolder(DataFolder);
            EnsureFolder(DevastadorFolder);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateLight();
            CreateArena();
            AbilityDefinition[] abilities = CreateAbilityAssets();
            GameObject player = CreatePlayer(abilities);
            GameObject camera = CreateCamera();
            CreateDummies();
            CreateEnemies();
            CreateWorldHealthBar(player.transform, player.GetComponent<Health>());
            CreateDamageNumbers();
            AimIndicator aimIndicator = CreateAimIndicator();
            CreateHud(out FloatingJoystick joystick, out CameraLookZone lookZone, out Transform hudRoot);
            CreateAbilityBar(hudRoot, player.GetComponent<AbilityCaster>(), player.GetComponent<PlayerDodge>(), aimIndicator);
            CreateStatusHud(hudRoot, player.GetComponent<Health>(), player.GetComponent<ClassResource>());
            CreateTargeting(hudRoot, player, hudRoot.GetComponentsInChildren<TapDetector>());
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

        private static GameObject CreatePlayer(AbilityDefinition[] abilities)
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
            root.AddComponent<PlayerDodge>();

            Health health = root.AddComponent<Health>();
            SetEnum(health, "team", (int)Team.Player);
            SetFloat(health, "maxHealth", 200f);

            // Rabia del Conquistador: empieza a 0, sube al golpear y al recibir
            // daño, baja poco a poco fuera de combate.
            ClassResource rage = root.AddComponent<ClassResource>();
            SetString(rage, "displayName", "Rabia");
            SetFloat(rage, "max", 100f);
            SetFloat(rage, "startValue", 0f);
            SetFloat(rage, "gainPerHitDealt", 5f);
            SetFloat(rage, "gainPerDamageTaken", 0.4f);
            SetFloat(rage, "combatTimeout", 5f);
            SetFloat(rage, "outOfCombatChangePerSecond", -8f);

            root.AddComponent<TargetSelector>();
            AddStatusEffects(root, 2.6f);

            AbilityCaster caster = root.AddComponent<AbilityCaster>();
            SetArray(caster, "slots", abilities);

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

            // Sin pantalla de muerte en el prototipo: reaparece en el origen a los 3 s.
            Respawner respawner = root.AddComponent<Respawner>();
            SetFloat(respawner, "delay", 3f);
            SetArray(respawner, "renderers", new Object[] { body.GetComponent<Renderer>(), marker.GetComponent<Renderer>() });
            SetArray(respawner, "behaviours", new Object[] { root.GetComponent<PlayerMovement>(), root.GetComponent<PlayerDodge>(), caster });

            return root;
        }

        // ------------------------------------------------------------------
        // Combate: habilidades, muñecos e indicador de apuntado
        // ------------------------------------------------------------------

        /// <summary>
        /// Crea (o reutiliza) los assets de habilidades de prueba en Data/ y devuelve
        /// las que van en la barra. Cada asset es una muestra de un tipo de habilidad
        /// (melé, proyectil, carga, zona) que luego reutilizarán todas las clases.
        /// </summary>
        private static AbilityDefinition[] CreateAbilityAssets()
        {
            var melee = LoadOrCreateAsset<MeleeAbility>("Melee");
            melee.displayName = "Golpe";
            melee.buttonColor = new Color(0.95f, 0.95f, 0.95f);
            melee.cooldown = 0.5f;
            melee.range = 2.5f;
            melee.aimMode = AimMode.Direction;
            melee.damage = 20f;
            melee.resourceCost = 0f;
            melee.resourceGain = 10f; // el básico genera Rabia
            melee.coneDegrees = 100f;
            melee.fxMaterial = CreateMaterial("FxMelee", new Color(1f, 1f, 0.8f));

            var projectile = LoadOrCreateAsset<ProjectileAbility>("Projectile");
            projectile.displayName = "Disparo";
            projectile.buttonColor = new Color(1f, 0.6f, 0.2f);
            projectile.cooldown = 2f;
            projectile.range = 14f;
            projectile.aimMode = AimMode.Direction;
            projectile.damage = 30f;
            projectile.resourceCost = 0f;
            projectile.resourceGain = 5f;
            projectile.speed = 18f;
            projectile.fxMaterial = CreateMaterial("FxProjectile", new Color(1f, 0.5f, 0.1f));

            // Carga con daño: tipo genérico (Salto, Carga con escudo...). No es la
            // esquiva, que tiene botón propio (PlayerDodge), y no va en la barra.
            var dash = LoadOrCreateAsset<DashAbility>("Dash");
            dash.displayName = "Carga";
            dash.buttonColor = new Color(0.3f, 0.85f, 1f);
            dash.cooldown = 4f;
            dash.range = 5f;
            dash.aimMode = AimMode.Direction;
            dash.damage = 15f;
            dash.resourceCost = 20f;
            dash.resourceGain = 0f;
            dash.duration = 0.2f;
            dash.fxMaterial = CreateMaterial("FxDash", new Color(0.5f, 0.9f, 1f));

            var blast = LoadOrCreateAsset<AreaBlastAbility>("AreaBlast");
            blast.displayName = "Estallido";
            blast.buttonColor = new Color(0.75f, 0.4f, 1f);
            blast.cooldown = 6f;
            blast.range = 8f;
            blast.aimMode = AimMode.Point;
            blast.damage = 40f;
            blast.resourceCost = 40f; // la fuerte gasta Rabia
            blast.resourceGain = 0f;
            blast.radius = 2.5f;
            blast.fxMaterial = CreateMaterial("FxBlast", new Color(0.7f, 0.3f, 1f));

            foreach (AbilityDefinition a in new AbilityDefinition[] { melee, projectile, dash, blast })
                EditorUtility.SetDirty(a);
            AssetDatabase.SaveAssets();

            return CreateDevastadorKit(melee);
        }

        /// <summary>
        /// Las 10 habilidades y 2 definitivas del Devastador como assets en
        /// Data/Devastador/, y la barra inicial: básico + 6 elegidas + Golpe del
        /// Conquistador. Las otras 4 y Furia quedan creadas para cambiarlas en
        /// el Inspector del AbilityCaster del jugador.
        /// </summary>
        private static AbilityDefinition[] CreateDevastadorKit(AbilityDefinition basic)
        {
            Material fx = CreateMaterial("FxDevastador", new Color(1f, 0.85f, 0.6f));
            Material fxControl = CreateMaterial("FxControl", new Color(1f, 0.9f, 0.2f));
            Material fxUltimate = CreateMaterial("FxUltimate", new Color(1f, 0.3f, 0.1f));

            MeleeAbility Melee(string file, string name, Color color, float dmg, float cone, float rng, float cd, float cost, float gain)
            {
                var a = LoadOrCreateAsset<MeleeAbility>("Devastador/" + file);
                a.displayName = name; a.buttonColor = color;
                a.damage = dmg; a.coneDegrees = cone; a.range = rng; a.cooldown = cd;
                a.resourceCost = cost; a.resourceGain = gain; a.aimMode = AimMode.Direction;
                a.hitCount = 1; a.executeThreshold = 0f; a.bleedDamagePerTick = 0f; a.bleedDuration = 0f;
                a.stunDuration = 0f; a.interrupts = false; a.fxMaterial = fx;
                return a;
            }

            Color gen = new Color(0.95f, 0.95f, 0.95f);   // generan Rabia
            Color spend = new Color(1f, 0.55f, 0.35f);    // gastan Rabia
            Color control = new Color(1f, 0.9f, 0.3f);    // controles

            var tajoGiratorio = Melee("TajoGiratorio", "Tajo giratorio", gen, 25f, 360f, 3f, 6f, 0f, 10f);
            var golpeDemoledor = Melee("GolpeDemoledor", "Golpe demoledor", spend, 70f, 60f, 2.5f, 8f, 30f, 0f);
            var ejecutar = Melee("Ejecutar", "Ejecutar", spend, 30f, 60f, 2.5f, 10f, 20f, 0f);
            ejecutar.executeThreshold = 0.3f; ejecutar.executeMultiplier = 3f;
            var desgarrar = Melee("Desgarrar", "Desgarrar", gen, 12f, 90f, 2.5f, 6f, 0f, 5f);
            desgarrar.bleedDamagePerTick = 8f; desgarrar.bleedInterval = 1f; desgarrar.bleedDuration = 6f;
            var rafaga = Melee("RafagaDeGolpes", "Ráfaga de golpes", spend, 15f, 60f, 2.5f, 8f, 15f, 0f);
            rafaga.hitCount = 3; rafaga.hitInterval = 0.2f;
            var hendidura = Melee("Hendidura", "Hendidura", gen, 35f, 90f, 3.5f, 5f, 0f, 5f);
            var conmocionador = Melee("GolpeConmocionador", "Golpe conmocionador", control, 15f, 60f, 2.5f, 15f, 25f, 0f);
            conmocionador.stunDuration = 4f; conmocionador.fxMaterial = fxControl;
            var patada = Melee("Patada", "Patada", control, 8f, 60f, 2.5f, 10f, 0f, 5f);
            patada.interrupts = true; patada.fxMaterial = fxControl;

            var salto = LoadOrCreateAsset<LeapAbility>("Devastador/Salto");
            salto.displayName = "Salto"; salto.buttonColor = gen;
            salto.damage = 30f; salto.range = 8f; salto.cooldown = 12f; salto.resourceCost = 0f; salto.resourceGain = 10f;
            salto.duration = 0.45f; salto.height = 2.5f; salto.radius = 2.5f; salto.aimMode = AimMode.Point; salto.fxMaterial = fx;

            var grito = LoadOrCreateAsset<WarCryAbility>("Devastador/GritoDeGuerra");
            grito.displayName = "Grito de guerra"; grito.buttonColor = control;
            grito.damage = 0f; grito.range = 6f; grito.cooldown = 20f; grito.resourceCost = 0f; grito.resourceGain = 25f;
            grito.slowFraction = 0.4f; grito.slowDuration = 4f; grito.aimMode = AimMode.Direction; grito.fxMaterial = fxControl;

            var furia = LoadOrCreateAsset<FuryAbility>("Devastador/Furia");
            furia.displayName = "Furia"; furia.buttonColor = new Color(1f, 0.25f, 0.2f);
            furia.damage = 0f; furia.range = 1f; furia.cooldown = 60f; furia.resourceCost = 0f; furia.resourceGain = 0f;
            furia.duration = 8f; furia.damageMultiplier = 1.3f; furia.lifesteal = 0.25f; furia.fxMaterial = fxUltimate;

            var golpeConquistador = LoadOrCreateAsset<ConquerorStrikeAbility>("Devastador/GolpeDelConquistador");
            golpeConquistador.displayName = "Golpe del Conquistador"; golpeConquistador.buttonColor = new Color(1f, 0.25f, 0.2f);
            golpeConquistador.damage = 80f; golpeConquistador.range = 3f; golpeConquistador.cooldown = 45f;
            golpeConquistador.resourceCost = 0f; golpeConquistador.resourceGain = 0f; golpeConquistador.faceLockDuration = 1f;
            golpeConquistador.chargeDuration = 1f; golpeConquistador.minimumResource = 30f; golpeConquistador.damagePerResource = 2f;
            golpeConquistador.aimMode = AimMode.Direction; golpeConquistador.fxMaterial = fxUltimate;

            var all = new AbilityDefinition[] { tajoGiratorio, golpeDemoledor, ejecutar, desgarrar, rafaga, hendidura, conmocionador, patada, salto, grito, furia, golpeConquistador };
            foreach (AbilityDefinition a in all) EditorUtility.SetDirty(a);
            AssetDatabase.SaveAssets();

            // 0 = básico, 1..6 = habilidades, 7 = definitiva.
            var slots = new AbilityDefinition[AbilityCaster.TotalSlots];
            slots[AbilityCaster.BasicSlot] = basic;
            slots[AbilityCaster.FirstAbilitySlot + 0] = tajoGiratorio;
            slots[AbilityCaster.FirstAbilitySlot + 1] = golpeDemoledor;
            slots[AbilityCaster.FirstAbilitySlot + 2] = desgarrar;
            slots[AbilityCaster.FirstAbilitySlot + 3] = conmocionador;
            slots[AbilityCaster.FirstAbilitySlot + 4] = patada;
            slots[AbilityCaster.FirstAbilitySlot + 5] = salto;
            slots[AbilityCaster.UltimateSlot] = golpeConquistador;
            return slots;
        }

        private static void CreateDummies()
        {
            Material normal = CreateMaterial("Dummy", new Color(0.85f, 0.25f, 0.25f));
            Material hit = CreateMaterial("DummyHit", Color.white);

            var parent = new GameObject("Dummies");
            Vector3[] positions = { new Vector3(0f, 0f, 6f), new Vector3(8f, 0f, -3f), new Vector3(-8f, 0f, -1f) };
            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 position = positions[i];
                var root = new GameObject($"Muñeco {i + 1}"); // el nombre sale en el panel de objetivo
                root.transform.SetParent(parent.transform, false);
                root.transform.position = position;
                root.transform.rotation = Quaternion.LookRotation(-position.normalized, Vector3.up);

                CharacterController controller = root.AddComponent<CharacterController>();
                controller.height = 2f;
                controller.radius = 0.5f;
                controller.center = new Vector3(0f, 1f, 0f);

                Health health = root.AddComponent<Health>();
                SetEnum(health, "team", (int)Team.Enemy);
                SetFloat(health, "maxHealth", 100f);

                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "Body";
                body.transform.SetParent(root.transform, false);
                body.transform.localPosition = new Vector3(0f, 1f, 0f);
                Object.DestroyImmediate(body.GetComponent<Collider>());
                Renderer bodyRenderer = body.GetComponent<Renderer>();
                bodyRenderer.sharedMaterial = normal;

                AddHitReactionAndRespawn(root, bodyRenderer, normal, hit, 2f, null);
                AddStatusEffects(root, 2.7f);
                root.AddComponent<TrainingDummy>();

                CreateWorldHealthBar(root.transform, health);
            }
        }

        /// <summary>Efectos de estado y sus marcadores sobre la cabeza (aturdido amarillo, ralentizado azul).</summary>
        private static void AddStatusEffects(GameObject root, float markerHeight)
        {
            root.AddComponent<StatusEffects>();
            Material stunMat = CreateMaterial("StatusStun", new Color(1f, 0.9f, 0.2f));
            Material slowMat = CreateMaterial("StatusSlow", new Color(0.3f, 0.6f, 1f));

            GameObject stun = CreateFlatCube("StunMarker", root.transform, stunMat, new Vector3(0.35f, 0.35f, 0.35f));
            stun.transform.localPosition = new Vector3(0f, markerHeight, 0f);
            stun.SetActive(false);
            GameObject slow = CreateFlatCube("SlowMarker", root.transform, slowMat, new Vector3(0.25f, 0.25f, 0.25f));
            slow.transform.localPosition = new Vector3(0.45f, markerHeight, 0f);
            slow.SetActive(false);

            StatusIndicator indicator = root.AddComponent<StatusIndicator>();
            SetReference(indicator, "stunMarker", stun.transform);
            SetReference(indicator, "slowMarker", slow.transform);
        }

        /// <summary>Parpadeo + empujón al recibir daño y reaparición tras morir, para muñecos y enemigos.</summary>
        private static void AddHitReactionAndRespawn(GameObject root, Renderer body, Material normal, Material hit, float respawnDelay, Behaviour brain)
        {
            HitReaction reaction = root.AddComponent<HitReaction>();
            SetReference(reaction, "body", body);
            SetReference(reaction, "normalMaterial", normal);
            SetReference(reaction, "hitMaterial", hit);

            Respawner respawner = root.AddComponent<Respawner>();
            SetFloat(respawner, "delay", respawnDelay);
            SetArray(respawner, "renderers", new Object[] { body });
            SetArray(respawner, "behaviours", brain != null ? new Object[] { brain } : new Object[0]);
        }

        /// <summary>
        /// Dos enemigos con IA: persiguen al jugador y atacan con aviso en el
        /// suelo. Se distinguen de los muñecos por el color (violeta) y por
        /// arrancar lejos, para que el jugador decida cuándo entrar.
        /// </summary>
        private static void CreateEnemies()
        {
            Material normal = CreateMaterial("Enemy", new Color(0.5f, 0.2f, 0.7f));
            Material hit = CreateMaterial("EnemyHit", Color.white);
            Material telegraphOutline = CreateMaterial("TelegraphOutline", new Color(0.6f, 0.05f, 0.05f));
            Material telegraphFill = CreateMaterial("TelegraphFill", new Color(1f, 0.2f, 0.1f));

            var parent = new GameObject("Enemies");
            Vector3[] positions = { new Vector3(14f, 0f, 10f), new Vector3(-13f, 0f, 12f) };
            for (int i = 0; i < positions.Length; i++)
            {
                var root = new GameObject($"Acechador {i + 1}");
                root.transform.SetParent(parent.transform, false);
                root.transform.position = positions[i];
                root.transform.rotation = Quaternion.LookRotation(-positions[i].normalized, Vector3.up);

                CharacterController controller = root.AddComponent<CharacterController>();
                controller.height = 2f;
                controller.radius = 0.5f;
                controller.center = new Vector3(0f, 1f, 0f);

                Health health = root.AddComponent<Health>();
                SetEnum(health, "team", (int)Team.Enemy);
                SetFloat(health, "maxHealth", 120f);

                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "Body";
                body.transform.SetParent(root.transform, false);
                body.transform.localPosition = new Vector3(0f, 1f, 0f);
                Object.DestroyImmediate(body.GetComponent<Collider>());
                Renderer bodyRenderer = body.GetComponent<Renderer>();
                bodyRenderer.sharedMaterial = normal;

                // Aviso de ataque: contorno fijo y relleno que crece, sueltos en el
                // mundo (hijos de un objeto propio) para no girar con el enemigo.
                var telegraphGo = new GameObject("AttackTelegraph");
                telegraphGo.transform.SetParent(root.transform, false);
                GameObject outline = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                outline.name = "Outline";
                outline.transform.SetParent(telegraphGo.transform, false);
                Object.DestroyImmediate(outline.GetComponent<Collider>());
                outline.GetComponent<Renderer>().sharedMaterial = telegraphOutline;
                GameObject fill = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                fill.name = "Fill";
                fill.transform.SetParent(telegraphGo.transform, false);
                Object.DestroyImmediate(fill.GetComponent<Collider>());
                fill.GetComponent<Renderer>().sharedMaterial = telegraphFill;
                AttackTelegraph telegraph = telegraphGo.AddComponent<AttackTelegraph>();
                SetReference(telegraph, "outline", outline.transform);
                SetReference(telegraph, "fill", fill.transform);

                AddStatusEffects(root, 2.7f);
                EnemyBrain brain = root.AddComponent<EnemyBrain>();
                SetReference(brain, "telegraph", telegraph);

                AddHitReactionAndRespawn(root, bodyRenderer, normal, hit, 5f, brain);
                CreateWorldHealthBar(root.transform, health);
            }
        }

        /// <summary>Barra de vida sobre la cabeza: dos cubos finos que miran a la cámara.</summary>
        private static void CreateWorldHealthBar(Transform owner, Health health)
        {
            Material barBg = CreateMaterial("HealthBarBackground", new Color(0.1f, 0.1f, 0.1f));
            Material barFill = CreateMaterial("HealthBarFill", new Color(0.2f, 0.9f, 0.3f));

            var bar = new GameObject("HealthBar");
            bar.transform.SetParent(owner, false);
            bar.transform.localPosition = new Vector3(0f, 2.4f, 0f);
            CreateFlatCube("Background", bar.transform, barBg, new Vector3(1.2f, 0.12f, 0.02f));
            GameObject fill = CreateFlatCube("Fill", bar.transform, barFill, new Vector3(1.2f, 0.1f, 0.02f));
            fill.transform.localPosition = new Vector3(0f, 0f, -0.01f);
            HealthBar healthBar = bar.AddComponent<HealthBar>();
            SetReference(healthBar, "health", health);
            SetReference(healthBar, "fill", fill.transform);
        }

        /// <summary>Números de daño flotantes: un solo escuchador para toda la escena.</summary>
        private static void CreateDamageNumbers()
        {
            var go = new GameObject("DamageNumbers");
            DamageNumberSpawner spawner = go.AddComponent<DamageNumberSpawner>();
            SetReference(spawner, "font", Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
        }

        private static AimIndicator CreateAimIndicator()
        {
            Material aimMat = CreateMaterial("AimIndicator", new Color(1f, 0.9f, 0.2f));

            var root = new GameObject("AimIndicator");
            AimIndicator indicator = root.AddComponent<AimIndicator>();

            GameObject strip = CreateFlatCube("DirectionStrip", root.transform, aimMat, new Vector3(0.6f, 0.02f, 4f));
            GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "PointDisc";
            disc.transform.SetParent(root.transform, false);
            Object.DestroyImmediate(disc.GetComponent<Collider>());
            disc.GetComponent<Renderer>().sharedMaterial = aimMat;

            SetReference(indicator, "directionStrip", strip.transform);
            SetReference(indicator, "pointDisc", disc.transform);
            return indicator;
        }

        private static GameObject CreateFlatCube(string name, Transform parent, Material material, Vector3 scale)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localScale = scale;
            Object.DestroyImmediate(cube.GetComponent<Collider>());
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
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

        private static void CreateHud(out FloatingJoystick joystick, out CameraLookZone lookZone, out Transform hudRoot)
        {
            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            hudRoot = canvasGo.transform;
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            // Sprite circular incluido en Unity; nos ahorra crear texturas.
            Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

            // Mitad izquierda: joystick flotante (y detector de toques para seleccionar objetivo).
            GameObject leftZone = CreateZone("JoystickZone", canvasGo.transform, new Vector2(0f, 0f), new Vector2(0.5f, 1f));
            leftZone.AddComponent<TapDetector>();
            joystick = leftZone.AddComponent<FloatingJoystick>();
            Image background = CreateImage("JoystickBackground", leftZone.transform, knob, 260f, new Color(1f, 1f, 1f, 0.35f));
            Image handle = CreateImage("JoystickHandle", background.transform, knob, 110f, new Color(1f, 1f, 1f, 0.8f));
            SetReference(joystick, "background", background.rectTransform);
            SetReference(joystick, "handle", handle.rectTransform);

            // Mitad derecha: giro de cámara (y detector de toques).
            GameObject rightZone = CreateZone("LookZone", canvasGo.transform, new Vector2(0.5f, 0f), new Vector2(1f, 1f));
            rightZone.AddComponent<TapDetector>();
            lookZone = rightZone.AddComponent<CameraLookZone>();
        }

        /// <summary>
        /// Barra de combate en la esquina inferior derecha: ataque básico grande,
        /// 6 habilidades en arco alrededor, definitiva y esquiva separadas. Todos
        /// los botones llevan <see cref="HudEditableElement"/> para poder moverse
        /// y escalarse desde el modo de edición del HUD. Van después de LookZone
        /// en la jerarquía para quedar por encima y capturar sus propios toques.
        /// </summary>
        private static void CreateAbilityBar(Transform hudRoot, AbilityCaster caster, PlayerDodge dodge, AimIndicator aimIndicator)
        {
            var bar = new GameObject("AbilityBar", typeof(RectTransform));
            bar.transform.SetParent(hudRoot, false);
            var barRt = bar.GetComponent<RectTransform>();
            barRt.anchorMin = barRt.anchorMax = barRt.pivot = new Vector2(1f, 0f); // esquina inferior derecha
            barRt.anchoredPosition = Vector2.zero;
            barRt.sizeDelta = Vector2.zero;

            // Posición (desde la esquina) y tamaño de cada ranura. Las 6 habilidades
            // van en un arco de radio 400 alrededor del básico, de izquierda (185°)
            // a arriba (75°), con 22° entre ellas para que no se toquen.
            Vector2 basicPos = new Vector2(-190f, 190f);
            var layout = new (Vector2 pos, float size)[AbilityCaster.TotalSlots];
            layout[AbilityCaster.BasicSlot] = (basicPos, 210f);
            for (int i = 0; i < AbilityCaster.AbilitySlotCount; i++)
            {
                float angle = (185f - 22f * i) * Mathf.Deg2Rad;
                Vector2 pos = basicPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 400f;
                layout[AbilityCaster.FirstAbilitySlot + i] = (new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y)), 140f);
            }
            layout[AbilityCaster.UltimateSlot] = (new Vector2(-780f, 140f), 160f);

            string[] names = { "Basico", "Habilidad1", "Habilidad2", "Habilidad3", "Habilidad4", "Habilidad5", "Habilidad6", "Definitiva" };
            for (int slot = 0; slot < layout.Length && slot < caster.SlotCount; slot++)
            {
                GameObject go = CreateRoundButton(names[slot], bar.transform, layout[slot].pos, layout[slot].size,
                    out Image background, out Image overlay, out Text label);

                AbilityButton button = go.AddComponent<AbilityButton>();
                SetReference(button, "caster", caster);
                SetInt(button, "slot", slot);
                SetReference(button, "aimIndicator", aimIndicator);
                SetReference(button, "background", background);
                SetReference(button, "cooldownOverlay", overlay);
                SetReference(button, "label", label);
            }

            // Esquiva: separada de las habilidades y cerca del pulgar (se usa mucho).
            {
                GameObject go = CreateRoundButton("Esquiva", bar.transform, new Vector2(-400f, 90f), 140f,
                    out Image background, out Image overlay, out Text label);

                DodgeButton button = go.AddComponent<DodgeButton>();
                SetReference(button, "dodge", dodge);
                SetReference(button, "background", background);
                SetReference(button, "cooldownOverlay", overlay);
                SetReference(button, "label", label);
            }

            // Cambiar de objetivo: pequeño, encima del básico y pegado al borde derecho.
            {
                GameObject go = CreateRoundButton("Objetivo", bar.transform, new Vector2(-90f, 400f), 110f,
                    out Image background, out Image overlay, out Text label);
                Object.DestroyImmediate(overlay.gameObject); // no tiene recarga

                CycleTargetButton button = go.AddComponent<CycleTargetButton>();
                SetReference(button, "selector", caster.GetComponent<TargetSelector>());
                SetReference(button, "background", background);
                SetReference(button, "label", label);
            }

            CreateHudLayoutEditor(hudRoot);
        }

        /// <summary>Botón "HUD" arriba a la derecha y panel de edición (oculto hasta pulsarlo).</summary>
        private static void CreateHudLayoutEditor(Transform hudRoot)
        {
            var root = new GameObject("HudLayoutEditor", typeof(RectTransform));
            root.transform.SetParent(hudRoot, false);
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            Button toggle = CreateTextButton("ToggleEdit", root.transform, new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(120f, 60f), "HUD");

            // Panel arriba en el centro con -, +, Restablecer, Listo y una pista.
            var panel = new GameObject("EditPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root.transform, false);
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = panelRt.anchorMax = panelRt.pivot = new Vector2(0.5f, 1f);
            panelRt.anchoredPosition = new Vector2(0f, -20f);
            panelRt.sizeDelta = new Vector2(900f, 150f);
            var panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.7f);
            panelImage.raycastTarget = true; // tapa lo que haya debajo

            Button smaller = CreateTextButton("Smaller", panel.transform, new Vector2(0f, 1f), new Vector2(30f, -20f), new Vector2(100f, 60f), "-");
            Button bigger = CreateTextButton("Bigger", panel.transform, new Vector2(0f, 1f), new Vector2(150f, -20f), new Vector2(100f, 60f), "+");
            Button reset = CreateTextButton("Reset", panel.transform, new Vector2(1f, 1f), new Vector2(-390f, -20f), new Vector2(220f, 60f), "Restablecer");
            Button done = CreateTextButton("Done", panel.transform, new Vector2(1f, 1f), new Vector2(-150f, -20f), new Vector2(120f, 60f), "Listo");

            var hintGo = new GameObject("Hint", typeof(RectTransform), typeof(Text));
            hintGo.transform.SetParent(panel.transform, false);
            var hintRt = hintGo.GetComponent<RectTransform>();
            hintRt.anchorMin = new Vector2(0f, 0f);
            hintRt.anchorMax = new Vector2(1f, 0f);
            hintRt.pivot = new Vector2(0.5f, 0f);
            hintRt.anchoredPosition = new Vector2(0f, 10f);
            hintRt.sizeDelta = new Vector2(-40f, 50f);
            var hint = hintGo.GetComponent<Text>();
            hint.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            hint.fontSize = 26;
            hint.alignment = TextAnchor.MiddleCenter;
            hint.color = Color.white;
            hint.raycastTarget = false;

            HudLayoutEditor editor = root.AddComponent<HudLayoutEditor>();
            SetReference(editor, "toggleButton", toggle);
            SetReference(editor, "editPanel", panel);
            SetReference(editor, "smallerButton", smaller);
            SetReference(editor, "biggerButton", bigger);
            SetReference(editor, "resetButton", reset);
            SetReference(editor, "doneButton", done);
            SetReference(editor, "hintLabel", hint);
            panel.SetActive(false);
        }

        /// <summary>Botón rectangular clásico con texto, anclado donde se pida.</summary>
        private static Button CreateTextButton(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, string text)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.85f);

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;
            var label = labelGo.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 30;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.1f, 0.1f, 0.1f);
            label.text = text;
            label.raycastTarget = false;

            return go.GetComponent<Button>();
        }

        /// <summary>
        /// Botón redondo anclado a la esquina inferior derecha, con sombra radial
        /// de recarga y etiqueta centrada. El componente de comportamiento lo
        /// añade quien llama.
        /// </summary>
        private static GameObject CreateRoundButton(string name, Transform parent, Vector2 position, float size,
            out Image background, out Image cooldownOverlay, out Text label)
        {
            Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = Vector2.one * size;

            background = go.GetComponent<Image>();
            background.sprite = knob;
            background.raycastTarget = true;

            // Sombra radial que se vacía con el enfriamiento.
            cooldownOverlay = CreateImage("Cooldown", go.transform, knob, size, new Color(0f, 0f, 0f, 0.6f));
            cooldownOverlay.type = Image.Type.Filled;
            cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
            cooldownOverlay.fillOrigin = (int)Image.Origin360.Top;
            cooldownOverlay.fillClockwise = false;
            cooldownOverlay.fillAmount = 0f;

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;
            label = labelGo.GetComponent<Text>();
            label.font = font;
            label.fontSize = 28;
            label.resizeTextForBestFit = true; // nombres largos ("Golpe conmocionador") se encogen
            label.resizeTextMinSize = 12;
            label.resizeTextMaxSize = 28;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.1f, 0.1f, 0.1f);
            label.raycastTarget = false;

            // Movible y escalable desde el modo de edición del HUD.
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 0.9f, 0.2f);
            outline.effectDistance = new Vector2(6f, -6f);
            outline.enabled = false;
            HudEditableElement editable = go.AddComponent<HudEditableElement>();
            SetReference(editable, "highlight", outline);

            return go;
        }

        /// <summary>
        /// Selección de objetivo: toque en la zona de cámara, anillo bajo el
        /// objetivo, panel arriba en el centro y botón "Cambiar".
        /// </summary>
        private static void CreateTargeting(Transform hudRoot, GameObject player, TapDetector[] detectors)
        {
            TargetSelector selector = player.GetComponent<TargetSelector>();

            TapToTarget tap = player.AddComponent<TapToTarget>();
            SetArray(tap, "detectors", detectors);
            SetReference(tap, "selector", selector);

            // Anillo en el suelo.
            Material ringMat = CreateMaterial("TargetRing", new Color(1f, 0.45f, 0.1f));
            var markerGo = new GameObject("TargetMarker");
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.SetParent(markerGo.transform, false);
            ring.transform.localScale = new Vector3(1.5f, 0.01f, 1.5f);
            Object.DestroyImmediate(ring.GetComponent<Collider>());
            ring.GetComponent<Renderer>().sharedMaterial = ringMat;
            TargetMarker marker = markerGo.AddComponent<TargetMarker>();
            SetReference(marker, "selector", selector);
            SetReference(marker, "ring", ring.transform);

            // Panel arriba en el centro + botón de cambiar objetivo a su derecha.
            var root = new GameObject("TargetHud", typeof(RectTransform));
            root.transform.SetParent(hudRoot, false);
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;

            var panel = new GameObject("Panel", typeof(RectTransform));
            panel.transform.SetParent(root.transform, false);
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = panelRt.anchorMax = panelRt.pivot = new Vector2(0.5f, 1f);
            panelRt.anchoredPosition = Vector2.zero;
            panelRt.sizeDelta = Vector2.zero;
            HudBar bar = CreateHudBar("TargetHealth", panel.transform, new Vector2(-250f, -40f), new Vector2(500f, 44f), new Color(0.9f, 0.25f, 0.2f));

            TargetHud hud = root.AddComponent<TargetHud>();
            SetReference(hud, "selector", selector);
            SetReference(hud, "panel", panel);
            SetReference(hud, "healthBar", bar);
            panel.SetActive(false);
        }

        /// <summary>Vida y recurso del jugador, arriba a la izquierda.</summary>
        private static void CreateStatusHud(Transform hudRoot, Health health, ClassResource resource)
        {
            var root = new GameObject("PlayerStatus", typeof(RectTransform));
            root.transform.SetParent(hudRoot, false);
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f); // esquina superior izquierda
            rt.anchoredPosition = new Vector2(40f, -40f);
            rt.sizeDelta = Vector2.zero;

            HudBar healthBar = CreateHudBar("HealthBar", root.transform, new Vector2(0f, 0f), new Vector2(420f, 44f), new Color(0.2f, 0.9f, 0.3f));
            HudBar resourceBar = CreateHudBar("ResourceBar", root.transform, new Vector2(0f, -56f), new Vector2(420f, 36f), new Color(0.95f, 0.3f, 0.2f));

            PlayerStatusHud hud = root.AddComponent<PlayerStatusHud>();
            SetReference(hud, "health", health);
            SetReference(hud, "resource", resource);
            SetReference(hud, "healthBar", healthBar);
            SetReference(hud, "resourceBar", resourceBar);
        }

        private static HudBar CreateHudBar(string name, Transform parent, Vector2 position, Vector2 size, Color fillColor)
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            var background = go.GetComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.6f);
            background.raycastTarget = false;

            var fillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillGo.transform.SetParent(go.transform, false);
            var fillRt = fillGo.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.offsetMin = new Vector2(3f, 3f);
            fillRt.offsetMax = new Vector2(-3f, -3f);
            var fill = fillGo.GetComponent<Image>();
            fill.color = fillColor;
            fill.raycastTarget = false;
            // Image.Filled necesita un sprite; el de UI blanco vale.
            fill.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            fill.fillAmount = 1f;

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;
            var label = labelGo.GetComponent<Text>();
            label.font = font;
            label.fontSize = Mathf.RoundToInt(size.y * 0.6f);
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.raycastTarget = false;

            HudBar bar = go.AddComponent<HudBar>();
            SetReference(bar, "fill", fill);
            SetReference(bar, "label", label);
            return bar;
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

        /// <summary>Carga el ScriptableObject de Data/ o lo crea si no existe.</summary>
        private static T LoadOrCreateAsset<T>(string name) where T : ScriptableObject
        {
            string path = $"{DataFolder}/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        /// <summary>Asigna un campo [SerializeField] privado sin exponerlo como público.</summary>
        private static void SetReference(Object target, string fieldName, Object value)
        {
            SetProperty(target, fieldName, p => p.objectReferenceValue = value);
        }

        private static void SetString(Object target, string fieldName, string value)
        {
            SetProperty(target, fieldName, p => p.stringValue = value);
        }

        private static void SetInt(Object target, string fieldName, int value)
        {
            SetProperty(target, fieldName, p => p.intValue = value);
        }

        private static void SetEnum(Object target, string fieldName, int value)
        {
            SetProperty(target, fieldName, p => p.enumValueIndex = value);
        }

        private static void SetFloat(Object target, string fieldName, float value)
        {
            SetProperty(target, fieldName, p => p.floatValue = value);
        }

        private static void SetArray(Object target, string fieldName, Object[] values)
        {
            SetProperty(target, fieldName, p =>
            {
                p.arraySize = values.Length;
                for (int i = 0; i < values.Length; i++)
                    p.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            });
        }

        private static void SetProperty(Object target, string fieldName, System.Action<SerializedProperty> assign)
        {
            var serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogError($"[Umbralis] No existe el campo '{fieldName}' en {target.GetType().Name}.");
                return;
            }
            assign(property);
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
