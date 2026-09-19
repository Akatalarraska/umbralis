# Prueba de la Fase 1 en el móvil

Todo lo de la Fase 1 está en código y compilado. Esta es la lista para dar la
fase por buena. Se prueba con `client/Builds/Umbralis.apk` (o el último que se
compile: incluye también lo de la Fase 2 que haya en ese momento).

```
adb install -r client\Builds\Umbralis.apk
```

Marca cada punto y anota al lado lo que no convenza: números, posiciones,
sensaciones. Con eso se ajusta antes de cerrar la fase.

## Ya confirmado (19-09-2026)

- [x] Joystick, cámara por arrastre, esquiva, Rabia al pegar, barra completa,
      modo edición del HUD, números de daño.
- [x] Los Acechadores atacan; Rabia al recibir daño.
- [x] Botón "Objetivo" en la barra (mejor que arriba).
- [x] Tocar a un enemigo lo selecciona (tras arreglar el umbral).

## Pendiente de probar

### Controles
- [ ] Pellizco en la mitad derecha acerca/aleja (2,5–12 m) sin girar la cámara.
      Al soltar un dedo, el otro sigue girando sin salto.
- [ ] Salto: mantener y arrastrar muestra el disco; soltar salta hasta él. Toque
      rápido salta delante del objetivo.
- [ ] Los nombres largos se leen en los botones (letra que se ajusta sola).

### Combate del Devastador
- [ ] Combo del diseño: Golpe conmocionador (cubo amarillo, 4 s quieto) →
      con ≥30 de Rabia, Golpe del Conquistador (1 s de barra, número grande).
- [ ] Rendimientos decrecientes: el mismo aturdimiento seguido dura 4 → 2 → 1 s
      → nada. Tras 15 s vuelve a 4.
- [ ] Golpe del Conquistador **sin** aturdir: si el enemigo se aleja durante la
      carga, falla (cubo pequeño), y la Rabia y la recarga se pierden.
      **¿Se siente justo o frustrante?**
- [ ] Patada mientras el Acechador anuncia (círculo rojo llenándose): el círculo
      desaparece y no pega.
- [ ] Desgarrar: números pequeños cada segundo durante 6 s.
- [ ] Furia (cámbiala en el Inspector por el Golpe del Conquistador si quieres
      probarla): 8 s de más daño y la vida sube al pegar.
- [ ] Botones naranja apagados hasta tener Rabia; se encienden solos.

### Enemigos
- [ ] El círculo rojo se lee bien y da tiempo a salir (1,5 s). ¿Demasiado fácil
      de esquivar? ¿Demasiado daño (15) o poco?
- [ ] Morir: reapareces en el origen a los 3 s.

### Medidor
- [ ] "DPS" abre el panel; pega 10 s con varias habilidades: el desglose suma
      100 % y el DPS se estabiliza. 5 s sin pegar → "(parado)".
- [ ] ¿El panel estorba al joystick o a la vista?

## Criterios de la fase (los cuatro de tareas.md)

- [ ] Se juega cómodo a dos manos en horizontal, sin fallos al pulsar botones.
      → ¿Qué botón se te escapa o se pulsa sin querer? Muévelo con "HUD" y
        apunta dónde lo dejas: esa será la disposición por defecto.
- [ ] Hay sensación de impacto al golpear.
      → ¿Falta empujón, parpadeo, tamaño de número, vibración?
- [ ] El combo aturdir + Golpe del Conquistador se siente técnico y satisfactorio.
- [ ] Apetece seguir pegando al muñeco cinco minutos sin ningún motivo.

## Anotado para más adelante (no bloquea la fase)

- Avisos sonoros de los telegrafiados (no hay audio todavía).
- Prioridad de selección configurable (más cercano / menos vida / jugadores).
- Velocidad de ataque en Furia (necesita animaciones).
- Invulnerabilidad durante la Embestida, si al probar hace falta.
- Pool de objetos para números de daño y efectos (40 personajes en pantalla).
