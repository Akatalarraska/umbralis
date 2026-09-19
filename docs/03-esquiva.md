# Fase 1 — Esquiva (Embestida)

La esquiva sale del sistema de habilidades y pasa a ser un botón propio, como
manda el diseño (barra = básico + 6 habilidades + definitiva + **esquiva**).

## Comportamiento

- Un toque en el botón "Esquiva" (abajo a la derecha, a la izquierda del grupo
  de habilidades) embiste 4 m en 0,18 s. Recarga de 3 s con sombra radial.
- Dispara **al apoyar el dedo**, no al soltar: una esquiva tiene que responder
  al instante.
- Dirección: hacia donde empuja el joystick; si el personaje está quieto, hacia
  donde mira.
- No hace daño ni se apunta. No se puede encadenar: mientras dura la embestida
  el botón se ignora.
- Los botones de habilidad quedan en 3 ranuras: Golpe, Disparo y Estallido.
  El asset `Dash` sigue existiendo como tipo genérico de "carga con daño"
  (renombrado a "Carga") para futuras habilidades como Salto o Carga con escudo.

## Piezas de código

- `Scripts/Player/PlayerDodge.cs` — distancia, duración, recarga y `TryDodge()`.
  Reutiliza `PlayerMovement.Dash`.
- `Scripts/TouchControls/DodgeButton.cs` — botón de un toque con sombra de recarga.
- `Editor/UmbralisSceneBuilder.cs` — añade `PlayerDodge` al jugador, crea el
  botón y comparte la construcción visual de botones (`CreateRoundButton`)
  con los de habilidad.

## Pasos en Unity

La escena y el APK ya están regenerados en batch mode con este cambio
(`client/Builds/Umbralis.apk`). Si prefieres hacerlo desde el editor:

1. **Umbralis → 1. Crear escena de prueba de combate** (regenera la escena con
   el botón de esquiva).
2. **Umbralis → 4. Compilar e instalar en el móvil** con el móvil conectado, o
   `adb install -r client\Builds\Umbralis.apk`.

## Qué comprobar en el móvil

- El botón "Esquiva" (azul claro) se alcanza con el pulgar derecho sin soltar
  el joystick con el izquierdo.
- Andando y pulsando, la embestida va hacia donde andas; quieto, hacia donde
  mira la cápsula.
- Responde al apoyar el dedo, sin retardo perceptible.
- Durante los 3 s de recarga el botón está sombreado y no hace nada.
- Ajustes de sensación en el Inspector del `Player`: `PlayerDodge.distance`,
  `duration`, `cooldown`. Posición del botón: `CreateAbilityBar` en el
  constructor de escena (`new Vector2(-660f, 140f)`).

## Pendiente para más adelante

- Invulnerabilidad durante la embestida (i-frames): se decide cuando haya
  enemigos que ataquen y se pueda probar si hace falta.
- Botones movibles y escalables: tarea de la barra de combate completa.
