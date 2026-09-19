# Fase 1 — Rabia, barra de combate completa, números de daño y objetivo

Cuatro tareas hechas de golpe para probarlas juntas en el móvil. Todas están
compiladas en `client/Builds/Umbralis.apk` (batch mode, sin errores).

```
adb install -r client\Builds\Umbralis.apk
```

## 1. Rabia y coste de recurso

- `Scripts/Combat/ClassResource.cs` — recurso de clase genérico. Configurado
  como Rabia en el jugador: 0–100, empieza a 0, **+5 por cada golpe que
  aciertas**, **+0,1 por cada punto de daño recibido**, y **−8/s** tras 5 s sin
  dar ni recibir daño.
- `AbilityDefinition` tiene ahora `damage` en la base, `resourceCost` y
  `resourceGain`. Valores de prueba: Golpe genera 10, Disparo genera 5, Carga
  cuesta 20, Estallido cuesta 40.
- `AbilityCaster.CanAfford(slot)`: sin Rabia suficiente el botón se ve apagado
  y el toque se ignora.
- `Health.TakeDamage` recibe al atacante y emite el evento estático
  `Health.AnyDamaged(atacante, víctima, cantidad)`, que usan la Rabia y los
  números flotantes (y usará el medidor de DPS).
- HUD arriba a la izquierda: vida (verde) y Rabia (roja) del jugador
  (`Scripts/HUD/HudBar.cs`, `PlayerStatusHud.cs`).

## 2. Barra de combate completa y botones editables

`AbilityCaster` tiene 8 ranuras fijas: **0 básico, 1–6 habilidades,
7 definitiva** (constantes `BasicSlot`, `FirstAbilitySlot`, `UltimateSlot`).
Las vacías se muestran apagadas para poder colocarlas.

Disposición inicial (esquina inferior derecha, unidades del canvas 1920×1080):

| Botón       | Posición     | Tamaño | Contenido de prueba |
|-------------|--------------|--------|---------------------|
| Básico      | (−190, 190)  | 210    | Golpe               |
| Habilidad 1–6 | arco de radio 400 alrededor del básico, de 185° a 75° | 140 | Disparo, Estallido, Carga, vacío ×3 |
| Definitiva  | (−780, 140)  | 160    | vacío               |
| Esquiva     | (−400, 90)   | 140    | Embestida           |

**Modo edición del HUD**: botón "HUD" arriba a la derecha.

- Mientras está activo los botones de combate no lanzan nada.
- Toca un botón para seleccionarlo (contorno amarillo) y arrástralo para moverlo.
- `−` / `+` lo encogen o agrandan (×0,6 a ×1,6, pasos de 0,1).
- "Restablecer" vuelve a la disposición inicial; "Listo" sale y **guarda en
  PlayerPrefs** (se conserva entre sesiones).
- Código: `Scripts/HUD/HudEditableElement.cs` (en cada botón) y
  `Scripts/HUD/HudLayoutEditor.cs`.

## 3. Números de daño flotantes

`Scripts/Combat/DamageNumberSpawner.cs` escucha `Health.AnyDamaged` y crea un
`DamageNumber` (TextMesh sin canvas) sobre la víctima: sube, frena y se
desvanece en 0,8 s. Blanco-amarillo sobre enemigos, rojo sobre el jugador;
el tamaño crece con la raíz del daño. El jugador tiene ahora también barra de
vida sobre la cabeza.

Pendiente de rendimiento: sin pool de objetos. Con 40 personajes en pantalla
habrá que reutilizar instancias.

## 4. Selección de objetivo

- **Tocar a un enemigo** en cualquier parte de la pantalla (toque corto, sin
  arrastre: menos de 0,3 s y 0,15 pulgadas, `TapDetector`) lo selecciona.
  Tocar el vacío no deselecciona.
- Botón redondo **"Objetivo"** en la barra, encima del básico: pasa al
  siguiente enemigo por cercanía (al más cercano si no había ninguno). Se mueve
  y escala como los demás.
- Anillo naranja bajo el objetivo y panel arriba en el centro con su nombre y
  vida. Se pierde al morir o a más de 30 m.
- El **toque rápido de una habilidad va al objetivo seleccionado** si está a
  tiro (alcance × 1,25); si no lo hay, al más cercano y lo selecciona.
- Código: `Scripts/Combat/TargetSelector.cs`, `TargetMarker.cs`,
  `Scripts/HUD/TargetHud.cs`, `Scripts/TouchControls/TapDetector.cs`,
  `TapToTarget.cs` y `CycleTargetButton.cs`.

## 5. Enemigo básico con IA y ataque telegrafiado

Dos "Acechadores" (violeta) lejos de los muñecos. `Scripts/Enemies/EnemyBrain.cs`:
esperan (12 m de aggro) → persiguen a 3,5 m/s → a 2,2 m anuncian el golpe
durante **1,5 s** con `AttackTelegraph` (contorno rojo oscuro + relleno que
crece hasta el borde) → golpe de 15 en un círculo de 2 m fijado al empezar el
aviso (se esquiva saliendo o con la Embestida) → 1,2 s de recuperación.
120 de vida; reaparecen a los 5 s.

Reparto de responsabilidades: `HitReaction` (parpadeo y empujón), `Respawner`
(ocultar, esperar, revivir; también en el jugador, 3 s), `TrainingDummy` solo
mueve el muñeco. La Rabia por daño recibido pasa a **0,4 por punto** (+6 por
golpe de Acechador).
- Pendiente del diseño: prioridad automática configurable (más cercano / menos
  vida / jugadores antes que NPC). Ahora es siempre "más cercano".

## Pasos en Unity

Nada manual: la escena se regenera con **Umbralis → 1. Crear escena de prueba
de combate** y el APK con **Umbralis → 3/4**. Ya están hechos en batch mode.

## Qué comprobar en el móvil

Rabia
- La barra roja sube al pegar al muñeco y baja sola tras unos segundos quieto.
- "Estallido" está apagado hasta tener 40 de Rabia; al usarlo baja 40.

Barra
- ¿Llegas cómodo con el pulgar derecho al básico, a las 6 del arco y a la
  esquiva sin soltar el joystick? ¿Alguna se pulsa sin querer al girar la cámara?
- Entra en "HUD", mueve y escala un botón, sal con "Listo", cierra la app y
  vuelve a abrirla: debe conservarse.

Números
- Se leen desde la distancia normal de cámara; no se pisan entre sí con golpes
  rápidos.

Objetivo
- Tocar un muñeco pone el anillo y el panel; "Cambiar" rota entre los tres.
- Con un muñeco seleccionado y otro más cerca, el toque corto en "Golpe" (si el
  seleccionado está a tiro) va al seleccionado, no al más cercano.
- Girar la cámara arrastrando no selecciona nada por accidente.
