# Fase 2 — Clase espejo: Templario / Juramentado

Tercera tarea de la Fase 2. Con ella la Fase 2 está en código. APK en
`client/Builds/Umbralis.apk`.

## Cómo funciona el espejo

Un solo juego de assets con los números; lo que cambia con la facción es solo
nombre, color de botón y material de efecto:

- `Core/FactionSettings.cs`: facción activa (Llama Blanca / Pacto Oscuro),
  guardada en PlayerPrefs, con evento `Changed`.
- `AbilityDefinition` tiene `hasMirror`, `mirrorName`, `mirrorColor`,
  `mirrorFxMaterial`. En ejecución todo usa `DisplayName`, `ButtonColor`,
  `FxMaterial`, que devuelven la versión de la facción activa. El medidor de
  daño, por tanto, muestra "Siega" o "Ajusticiar" según la facción, sobre el
  mismo asset.
- `SpecializationDefinition` y `ClassDefinition` tienen `mirrorName`; la clase
  además `resourceMirrorName` (Fervor / Tributo) y color de cuerpo por facción.

## Clases por datos

`Abilities/ClassDefinition.cs` (`Data/Clases/`): nombre y espejo, recurso
(nombre, máximo, inicio, ganancia por golpe y por daño recibido, cambio fuera
de combate), dos especializaciones y color del cuerpo. `Player/PlayerClass.cs`
la aplica: configura `ClassResource`, entrega las especializaciones al
`SpecializationSwitcher` y pinta la cápsula.

Botones de prototipo abajo en el centro: **"Clase"** (Conquistador ↔
Templario; solo fuera de combate) y **"Facción"** (siempre: solo cambia
nombres y colores, ni números ni recargas).

## Templario / Juramentado

Recurso **Fervor / Tributo**: como la Rabia pero con más ganancia por daño
recibido (0,5 por punto: "también se genera protegiendo"). Cuerpo blanco
hueso (Llama) o violeta ceniza (Pacto). Efectos: llama dorada / ceniza verde.

### Guardallama / Guardatumbas (tanque)

Pasiva **Fervor curativo** (`ZealPassive`): parte del daño que hace le cura,
del 10 % con la vida llena al 40 % al borde de la muerte.

| Llama Blanca | Pacto Oscuro (*provisional*) | Tipo | Valores |
|---|---|---|---|
| Golpe llameante | Golpe sepulcral | Melee | 22, 90°, 2,5 m, 4 s, +10, +30 amenaza |
| Estallido de brasas | Estallido de ceniza | Melee | 25, 360°, 3 m, 6 s, +10, +20 amenaza |
| Anillo de brasas | Anillo de huesos | AreaBlast | 30, radio 3, 6 m, 8 s, −20 |
| Condena | Sentencia | Taunt | 15 m, 10 s, +10 |
| Reprimenda | Reprensión | Melee | 15, interrumpe, 8 s, +5 |
| Desafío | Desafío de ultratumba | Taunt | radio 8, 20 s, +15 |
| Égida | Sudario | SelfBuff | 40 % menos daño 6 s, 25 s |
| Llama interior | Fuego fatuo | SelfBuff | espinas 50 % 6 s, 20 s, −20 |
| Voto de amparo | Pacto de amparo | Intercept | 15 m, 20 s, 50 % 6 s |
| Avance sagrado | Avance profano | Dash | 15, 8 m, aturde 1,5 s, 12 s, +10 |

Definitivas: Llama inquebrantable / Legión ancestral (4 s sin morir, 90 s) y
Encarnación de la llama / Coloso de los caídos (30 % menos daño a 8 m, 8 s, 60 s).

### Inquisidor / Segador (daño)

Pasiva **Brasas del juicio / Almas errantes** (`EmberPassive`): el 30 % de los
golpes dejan una brasa (disco naranja; alma verde en el Pacto) bajo la
víctima; pisarla cura un 5 % y da 10 de recurso. Duran 10 s.

| Llama Blanca | Pacto Oscuro | Tipo | Valores |
|---|---|---|---|
| Tajo ardiente | Tajo sepulcral (*prov.*) | Melee | 25, 90°, 4 s, +10 |
| Ajusticiar | Siega | Melee | 30 (×3 bajo 30 %), 10 s, −20 |
| Marca del hereje | Marca del condenado | Melee | 10 + 8/s × 6 s, 6 s, +5 |
| Onda purificadora | Onda profana (*prov.*) | AreaBlast | 35, radio 2,5, 8 m, 8 s, −25 |
| Torbellino de brasas | Torbellino de cenizas (*prov.*) | Melee | 12 ×3, 360°, 8 s, −15 |
| Persecución | Acecho (*prov.*) | Dash | 20, 8 m, 10 s, +10 |
| Cadenas de penitencia | Cadenas del pacto (*prov.*) | WarCry | ralentiza 50 % 4 s, radio 6, 15 s, +10 |
| Castigo cegador | Castigo sombrío (*prov.*) | Melee | 15, aturde 3 s, 15 s, −25 |
| Silencio del tribunal | Silencio de la tumba (*prov.*) | Melee | 8, interrumpe, 10 s, +5 |
| Penitencia | Precio del pacto | Fury | daño ×1,2 5 s, 30 s, −30 |

Definitivas: Hoguera / Marea de difuntos (120 en radio 4, 60 s) y Auto de fe /
Juicio de la tumba (150 a un objetivo, 60 s, −50).

Los nombres marcados *provisional* no están en el GDD: los inventé para que
el espejo estuviera completo. Cámbialos en `UmbralisSceneBuilder.Classes.cs`
(o en el asset) cuando tengas los buenos.

## Qué comprobar en el móvil

- "Facción" en mitad de un combate: los botones cambian de nombre y color al
  instante, la Rabia sigue igual, las recargas no se tocan.
- "Clase" fuera de combate → Templario: la cápsula se vuelve blanca, el
  recurso se llama Fervor. Con el Pacto: violeta y Tributo.
- Guardallama: la vida sube al pegar, más cuanto menos tienes.
- Inquisidor: aparecen discos naranja bajo los enemigos; al pisarlos, curación
  y recurso. En el Pacto son verdes.
- Ajusticiar (Llama) y Siega (Pacto) hacen exactamente el mismo daño.
