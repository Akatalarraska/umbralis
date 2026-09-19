using UnityEditor;
using UnityEngine;
using Umbralis.Abilities;
using Umbralis.Passives;

namespace Umbralis.EditorTools
{
    /// <summary>
    /// Parte del constructor de escena que crea las clases por datos: el
    /// Conquistador (Devastador + Baluarte) y la clase espejo Templario /
    /// Juramentado (Guardallama / Guardatumbas + Inquisidor / Segador).
    /// Los nombres del Pacto que el GDD no fija son provisionales.
    /// </summary>
    public static partial class UmbralisSceneBuilder
    {
        private const string TemplarFolder = DataFolder + "/Templario";
        private const string ClassesFolder = DataFolder + "/Clases";

        private static ClassDefinition CreateConquistadorClass(SpecializationDefinition devastador, SpecializationDefinition baluarte)
        {
            var c = LoadOrCreateAsset<ClassDefinition>("Clases/Conquistador");
            c.displayName = "Conquistador"; c.mirrorName = "";
            c.description = "Guerrero de primera línea que se hace más peligroso cuanto más dura el combate. Armadura pesada.";
            c.resourceName = "Rabia"; c.resourceMirrorName = "";
            c.resourceMax = 100f; c.resourceStart = 0f; c.resourceGainPerHit = 5f; c.resourceGainPerDamageTaken = 0.4f; c.resourceOutOfCombatChange = -8f;
            c.specializations = new[] { devastador, baluarte };
            c.bodyColor = new Color(0.25f, 0.55f, 0.95f); c.mirrorBodyColor = new Color(0.25f, 0.55f, 0.95f);
            EditorUtility.SetDirty(c);
            AssetDatabase.SaveAssets();
            return c;
        }

        /// <summary>Templario (Llama Blanca) / Juramentado (Pacto Oscuro): mismos assets, mismos números.</summary>
        private static ClassDefinition CreateTemplarClass(AbilityDefinition basic)
        {
            EnsureFolder(TemplarFolder);
            EnsureFolder(ClassesFolder);

            // Efectos: llama (blanco-dorado) para la Llama Blanca; ceniza (verde espectral) para el Pacto.
            Material fxFlame = CreateMaterial("FxLlama", new Color(1f, 0.85f, 0.45f));
            Material fxAsh = CreateMaterial("FxCeniza", new Color(0.45f, 0.95f, 0.6f));
            Material fxControl = CreateMaterial("FxControl", new Color(1f, 0.9f, 0.2f));
            Material fxUltimate = CreateMaterial("FxUltimate", new Color(1f, 0.3f, 0.1f));

            Color gen = new Color(1f, 0.97f, 0.85f), genM = new Color(0.8f, 0.95f, 0.85f);   // generan
            Color spend = new Color(1f, 0.6f, 0.3f), spendM = new Color(0.55f, 0.75f, 0.45f); // gastan
            Color control = new Color(1f, 0.9f, 0.3f), controlM = new Color(0.6f, 0.9f, 0.5f);
            Color ult = new Color(1f, 0.25f, 0.2f), ultM = new Color(0.4f, 0.2f, 0.7f);

            // Cada helper deja los números y pone nombre/color/efecto para las dos facciones.
            T Mirror<T>(T a, string name, string mirror, Color color, Color mirrorColor, Material fx, Material mirrorFx) where T : AbilityDefinition
            {
                a.displayName = name; a.buttonColor = color; a.fxMaterial = fx;
                a.hasMirror = true; a.mirrorName = mirror; a.mirrorColor = mirrorColor; a.mirrorFxMaterial = mirrorFx;
                return a;
            }
            MeleeAbility Melee(string file, float dmg, float cone, float rng, float cd, float cost, float gain)
            {
                var a = LoadOrCreateAsset<MeleeAbility>("Templario/" + file);
                a.damage = dmg; a.coneDegrees = cone; a.range = rng; a.cooldown = cd; a.resourceCost = cost; a.resourceGain = gain;
                a.aimMode = AimMode.Direction; a.hitCount = 1; a.executeThreshold = 0f; a.bleedDamagePerTick = 0f; a.bleedDuration = 0f;
                a.stunDuration = 0f; a.interrupts = false; a.threat = 0f; a.bonusPerBulwarkCharge = 0f;
                return a;
            }
            SelfBuffAbility Buff(string file, float cd, float cost, float duration)
            {
                var a = LoadOrCreateAsset<SelfBuffAbility>("Templario/" + file);
                a.damage = 0f; a.range = 1f; a.cooldown = cd; a.resourceCost = cost; a.resourceGain = 0f; a.aimMode = AimMode.Direction;
                a.duration = duration; a.radius = 0f; a.damageReduction = 0f; a.frontalDamageReduction = 0f; a.thorns = 0f; a.preventDeath = false;
                return a;
            }
            TauntAbility Taunt(string file, float rng, float radius, float cd, float gain, float threat)
            {
                var a = LoadOrCreateAsset<TauntAbility>("Templario/" + file);
                a.damage = 0f; a.range = rng; a.radius = radius; a.cooldown = cd; a.resourceCost = 0f; a.resourceGain = gain;
                a.duration = 4f; a.threat = threat; a.aimMode = AimMode.Direction;
                return a;
            }
            AreaBlastAbility Blast(string file, float dmg, float radius, float rng, float cd, float cost)
            {
                var a = LoadOrCreateAsset<AreaBlastAbility>("Templario/" + file);
                a.damage = dmg; a.radius = radius; a.range = rng; a.cooldown = cd; a.resourceCost = cost; a.resourceGain = 0f; a.aimMode = AimMode.Point;
                return a;
            }

            // ---------------- Guardallama / Guardatumbas (tanque) ----------------
            var golpeLlameante = Mirror(Melee("GolpeLlameante", 22f, 90f, 2.5f, 4f, 0f, 10f), "Golpe llameante", "Golpe sepulcral", gen, genM, fxFlame, fxAsh);
            golpeLlameante.threat = 30f;
            var estallidoBrasas = Mirror(Melee("EstallidoDeBrasas", 25f, 360f, 3f, 6f, 0f, 10f), "Estallido de brasas", "Estallido de ceniza", gen, genM, fxFlame, fxAsh);
            estallidoBrasas.threat = 20f;
            var anilloBrasas = Mirror(Blast("AnilloDeBrasas", 30f, 3f, 6f, 8f, 20f), "Anillo de brasas", "Anillo de huesos", spend, spendM, fxFlame, fxAsh);
            var condena = Mirror(Taunt("Condena", 15f, 0f, 10f, 10f, 50f), "Condena", "Sentencia", control, controlM, fxControl, fxAsh);
            var reprimenda = Mirror(Melee("Reprimenda", 15f, 60f, 2.5f, 8f, 0f, 5f), "Reprimenda", "Reprensión", control, controlM, fxControl, fxAsh);
            reprimenda.interrupts = true;
            var desafio = Mirror(Taunt("Desafio", 8f, 8f, 20f, 15f, 60f), "Desafío", "Desafío de ultratumba", control, controlM, fxControl, fxAsh);
            var egida = Mirror(Buff("Egida", 25f, 0f, 6f), "Égida", "Sudario", gen, genM, fxFlame, fxAsh);
            egida.damageReduction = 0.4f;
            var llamaInterior = Mirror(Buff("LlamaInterior", 20f, 20f, 6f), "Llama interior", "Fuego fatuo", spend, spendM, fxFlame, fxAsh);
            llamaInterior.thorns = 0.5f;
            var votoAmparo = LoadOrCreateAsset<InterceptAbility>("Templario/VotoDeAmparo");
            votoAmparo.damage = 0f; votoAmparo.range = 15f; votoAmparo.cooldown = 20f; votoAmparo.resourceCost = 0f; votoAmparo.resourceGain = 5f;
            votoAmparo.leapDuration = 0.4f; votoAmparo.duration = 6f; votoAmparo.share = 0.5f; votoAmparo.aimMode = AimMode.Direction;
            Mirror(votoAmparo, "Voto de amparo", "Pacto de amparo", gen, genM, fxFlame, fxAsh);
            var avanceSagrado = LoadOrCreateAsset<DashAbility>("Templario/AvanceSagrado");
            avanceSagrado.damage = 15f; avanceSagrado.range = 8f; avanceSagrado.cooldown = 12f; avanceSagrado.resourceCost = 0f; avanceSagrado.resourceGain = 10f;
            avanceSagrado.duration = 0.3f; avanceSagrado.hitRadius = 0.9f; avanceSagrado.stunDuration = 1.5f; avanceSagrado.aimMode = AimMode.Direction;
            Mirror(avanceSagrado, "Avance sagrado", "Avance profano", control, controlM, fxControl, fxAsh);

            var llamaInquebrantable = Mirror(Buff("LlamaInquebrantable", 90f, 0f, 4f), "Llama inquebrantable", "Legión ancestral", ult, ultM, fxUltimate, fxAsh);
            llamaInquebrantable.preventDeath = true;
            var encarnacion = Mirror(Buff("EncarnacionDeLaLlama", 60f, 0f, 8f), "Encarnación de la llama", "Coloso de los caídos", ult, ultM, fxUltimate, fxAsh);
            encarnacion.radius = 8f; encarnacion.damageReduction = 0.3f;

            var zeal = LoadOrCreateAsset<ZealPassiveDefinition>("Pasivas/FervorCurativo");
            zeal.displayName = "Fervor curativo";
            zeal.description = "Parte del daño que hace le cura: del 10 % con la vida llena al 40 % al borde de la muerte.";
            zeal.minHealFraction = 0.1f; zeal.maxHealFraction = 0.4f;

            var guardallama = LoadOrCreateAsset<SpecializationDefinition>("Especializaciones/Guardallama");
            guardallama.displayName = "Guardallama"; guardallama.mirrorName = "Guardatumbas";
            guardallama.description = "Tanque del Templario / Juramentado.";
            guardallama.basicAttack = basic;
            guardallama.abilities = new AbilityDefinition[] { golpeLlameante, estallidoBrasas, anilloBrasas, condena, reprimenda, desafio, egida, llamaInterior, votoAmparo, avanceSagrado };
            guardallama.ultimates = new AbilityDefinition[] { llamaInquebrantable, encarnacion };
            guardallama.passive = zeal;
            guardallama.defaultAbilityIndices = new[] { 0, 1, 2, 3, 4, 9 }; // Golpe llameante, Estallido, Anillo, Condena, Reprimenda, Avance sagrado
            guardallama.defaultUltimateIndex = 0;

            // ---------------- Inquisidor / Segador (daño) ----------------
            var tajoArdiente = Mirror(Melee("TajoArdiente", 25f, 90f, 2.5f, 4f, 0f, 10f), "Tajo ardiente", "Tajo sepulcral", gen, genM, fxFlame, fxAsh);
            var ajusticiar = Mirror(Melee("Ajusticiar", 30f, 60f, 2.5f, 10f, 20f, 0f), "Ajusticiar", "Siega", spend, spendM, fxFlame, fxAsh);
            ajusticiar.executeThreshold = 0.3f; ajusticiar.executeMultiplier = 3f;
            var marca = Mirror(Melee("MarcaDelHereje", 10f, 90f, 2.5f, 6f, 0f, 5f), "Marca del hereje", "Marca del condenado", gen, genM, fxFlame, fxAsh);
            marca.bleedDamagePerTick = 8f; marca.bleedInterval = 1f; marca.bleedDuration = 6f;
            var onda = Mirror(Blast("OndaPurificadora", 35f, 2.5f, 8f, 8f, 25f), "Onda purificadora", "Onda profana", spend, spendM, fxFlame, fxAsh);
            var torbellino = Mirror(Melee("TorbellinoDeBrasas", 12f, 360f, 3f, 8f, 15f, 0f), "Torbellino de brasas", "Torbellino de cenizas", spend, spendM, fxFlame, fxAsh);
            torbellino.hitCount = 3; torbellino.hitInterval = 0.2f;
            var persecucion = LoadOrCreateAsset<DashAbility>("Templario/Persecucion");
            persecucion.damage = 20f; persecucion.range = 8f; persecucion.cooldown = 10f; persecucion.resourceCost = 0f; persecucion.resourceGain = 10f;
            persecucion.duration = 0.25f; persecucion.hitRadius = 0.8f; persecucion.stunDuration = 0f; persecucion.aimMode = AimMode.Direction;
            Mirror(persecucion, "Persecución", "Acecho", gen, genM, fxFlame, fxAsh);
            var cadenas = LoadOrCreateAsset<WarCryAbility>("Templario/CadenasDePenitencia");
            cadenas.damage = 0f; cadenas.range = 6f; cadenas.cooldown = 15f; cadenas.resourceCost = 0f; cadenas.resourceGain = 10f;
            cadenas.slowFraction = 0.5f; cadenas.slowDuration = 4f; cadenas.aimMode = AimMode.Direction;
            Mirror(cadenas, "Cadenas de penitencia", "Cadenas del pacto", control, controlM, fxControl, fxAsh);
            var castigo = Mirror(Melee("CastigoCegador", 15f, 60f, 2.5f, 15f, 25f, 0f), "Castigo cegador", "Castigo sombrío", control, controlM, fxControl, fxAsh);
            castigo.stunDuration = 3f;
            var silencio = Mirror(Melee("SilencioDelTribunal", 8f, 60f, 2.5f, 10f, 0f, 5f), "Silencio del tribunal", "Silencio de la tumba", control, controlM, fxControl, fxAsh);
            silencio.interrupts = true;
            var penitencia = LoadOrCreateAsset<FuryAbility>("Templario/Penitencia");
            penitencia.damage = 0f; penitencia.range = 1f; penitencia.cooldown = 30f; penitencia.resourceCost = 30f; penitencia.resourceGain = 0f;
            penitencia.duration = 5f; penitencia.damageMultiplier = 1.2f; penitencia.lifesteal = 0f; penitencia.aimMode = AimMode.Direction;
            Mirror(penitencia, "Penitencia", "Precio del pacto", spend, spendM, fxFlame, fxAsh);

            var hoguera = Mirror(Blast("Hoguera", 120f, 4f, 8f, 60f, 0f), "Hoguera", "Marea de difuntos", ult, ultM, fxUltimate, fxAsh);
            var autoDeFe = Mirror(Melee("AutoDeFe", 150f, 30f, 3f, 60f, 50f, 0f), "Auto de fe", "Juicio de la tumba", ult, ultM, fxUltimate, fxAsh);

            var embers = LoadOrCreateAsset<EmberPassiveDefinition>("Pasivas/BrasasDelJuicio");
            embers.displayName = "Brasas del juicio";
            embers.description = "Algunos golpes (30 %) dejan una brasa bajo la víctima; pisarla cura un 5 % y da 10 de recurso.";
            embers.chance = 0.3f; embers.healFraction = 0.05f; embers.resourceGain = 10f; embers.lifetime = 10f;
            embers.material = CreateMaterial("Brasa", new Color(1f, 0.5f, 0.1f));
            embers.mirrorMaterial = CreateMaterial("Alma", new Color(0.5f, 1f, 0.7f));

            var inquisidor = LoadOrCreateAsset<SpecializationDefinition>("Especializaciones/Inquisidor");
            inquisidor.displayName = "Inquisidor"; inquisidor.mirrorName = "Segador";
            inquisidor.description = "Daño del Templario / Juramentado.";
            inquisidor.basicAttack = basic;
            inquisidor.abilities = new AbilityDefinition[] { tajoArdiente, ajusticiar, marca, onda, torbellino, persecucion, cadenas, castigo, silencio, penitencia };
            inquisidor.ultimates = new AbilityDefinition[] { hoguera, autoDeFe };
            inquisidor.passive = embers;
            inquisidor.defaultAbilityIndices = new[] { 0, 1, 2, 4, 7, 8 }; // Tajo ardiente, Ajusticiar, Marca, Torbellino, Castigo, Silencio
            inquisidor.defaultUltimateIndex = 1;

            foreach (Object o in new Object[] { golpeLlameante, estallidoBrasas, anilloBrasas, condena, reprimenda, desafio, egida, llamaInterior, votoAmparo, avanceSagrado,
                llamaInquebrantable, encarnacion, zeal, guardallama, tajoArdiente, ajusticiar, marca, onda, torbellino, persecucion, cadenas, castigo, silencio, penitencia,
                hoguera, autoDeFe, embers, inquisidor })
                EditorUtility.SetDirty(o);

            var c = LoadOrCreateAsset<ClassDefinition>("Clases/Templario");
            c.displayName = "Templario"; c.mirrorName = "Juramentado";
            c.description = "Armadura pesada. Recurso Fervor / Tributo, que también se genera protegiendo.";
            c.resourceName = "Fervor"; c.resourceMirrorName = "Tributo";
            c.resourceMax = 100f; c.resourceStart = 0f; c.resourceGainPerHit = 5f; c.resourceGainPerDamageTaken = 0.5f; c.resourceOutOfCombatChange = -8f;
            c.specializations = new[] { guardallama, inquisidor };
            c.bodyColor = new Color(0.95f, 0.9f, 0.75f);      // blanco hueso y dorado
            c.mirrorBodyColor = new Color(0.3f, 0.2f, 0.45f); // gris ceniza y violeta espectral
            EditorUtility.SetDirty(c);
            AssetDatabase.SaveAssets();
            return c;
        }
    }
}
