# Fase 1 — Pellizco para zoom y medidor de daño

Las dos últimas tareas de la Fase 1. APK en `client/Builds/Umbralis.apk`.

## Pellizco para zoom

`Scripts/TouchControls/PinchZoom.cs`, en la misma zona derecha que el giro de
cámara. Con dos dedos, separar acerca la cámara y juntar la aleja, entre 2,5 y
12 m (`ThirdPersonCamera.distanceLimits`; `metersPerScreenWidth` = 12 m por
ancho de pantalla de separación). Mientras hay dos dedos, el giro se pausa para
no girar sin querer. Un solo dedo sigue girando como siempre.

## Medidor de daño (muñeco de entrenamiento)

- `Scripts/Combat/DamageMeter.cs` en el jugador: total, DPS y desglose por
  habilidad. La sesión empieza con el primer golpe y se congela tras 5 s sin
  pegar; el siguiente golpe empieza otra. Solo mide el daño del propio jugador.
- Para el desglose, todo daño lleva ahora el **nombre de la habilidad**:
  `Health.DealDamage(víctima, daño, dirección, origen)` y el evento
  `Health.AnyDamaged(atacante, víctima, cantidad, origen)`. Los sangrados se
  listan aparte ("Desgarrar (sangrado)").
- HUD: botón **"DPS"** junto a las barras de vida y Rabia abre un panel
  plegable debajo con DPS, total, duración y hasta 8 filas con porcentaje;
  "Reiniciar" lo pone a cero. El panel no captura toques: el joystick sigue
  funcionando debajo.

## Qué comprobar en el móvil

- Pellizco en la mitad derecha: acerca y aleja sin girar; al soltar un dedo el
  otro vuelve a girar sin salto brusco.
- "DPS": pega al muñeco 10 s con varias habilidades; el desglose debe sumar
  100 % y el DPS estabilizarse. Deja de pegar 5 s: se queda "(parado)".
- ¿El panel molesta al joystick o a la vista? Es plegable, pero dime si su
  sitio (bajo las barras) estorba.
