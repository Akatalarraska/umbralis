# Fase 1 — Efectos de estado, kit del Devastador y definitivas

Tres tareas juntas: rendimientos decrecientes, las 10 habilidades del
Devastador y las dos definitivas. APK en `client/Builds/Umbralis.apk`.

## Efectos de estado (`Scripts/Combat/StatusEffects.cs`)

En jugador, muñecos y enemigos. Aturdimiento, ralentización, sangrados y
potenciación. Quien se mueve o lanza pregunta aquí:

- `EnemyBrain`: aturdido no se mueve ni ataca; ralentizado persigue más lento;
  al ser aturdido o interrumpido **cancela el golpe que estaba anunciando** y
  pierde el turno (pasa a recuperación).
- `PlayerMovement`: aturdido o anclado (`Rooted`, cargando una definitiva) el
  joystick no manda; la velocidad lleva el multiplicador de ralentización.
- `AbilityCaster`: no lanza aturdido, canalizando ni en el aire. Todo daño pasa
  por `Health.DealDamage(víctima, daño, dirección)`, que aplica el multiplicador
  de daño y el robo de vida de la potenciación.

**Rendimientos decrecientes** en aturdimientos: sobre un mismo objetivo, la
primera aplicación dura el 100 %, la segunda el 50 %, la tercera el 25 % y la
cuarta no entra (inmune). La cuenta se reinicia tras 15 s sin recibir ese
control. Están hechos para poder aplicarlos a cualquier control que se añada.

Se ven (`StatusIndicator`): cubo amarillo girando sobre la cabeza = aturdido;
cubo azul = ralentizado.

## Kit del Devastador (`Data/Devastador/`)

Todas son assets de tipos genéricos. `MeleeAbility` ha crecido con efectos
opcionales (varios golpes, ejecutar, sangrado, aturdir, interrumpir) y cubre
8 de las 10; `LeapAbility` y `WarCryAbility` son nuevos.

| Habilidad | Tipo | Daño | Cono | Alcance | Recarga | Rabia | Efecto |
|---|---|---|---|---|---|---|---|
| Golpe (básico) | Melee | 20 | 100° | 2,5 | 0,5 s | +10 | — |
| Tajo giratorio | Melee | 25 | 360° | 3 | 6 s | +10 | — |
| Golpe demoledor | Melee | 70 | 60° | 2,5 | 8 s | −30 | — |
| Ejecutar | Melee | 30 (×3 si <30 % vida) | 60° | 2,5 | 10 s | −20 | — |
| Desgarrar | Melee | 12 | 90° | 2,5 | 6 s | +5 | sangrado 8/s durante 6 s |
| Ráfaga de golpes | Melee | 15 ×3 | 60° | 2,5 | 8 s | −15 | 3 golpes cada 0,2 s |
| Hendidura | Melee | 35 | 90° | 3,5 | 5 s | +5 | — |
| Golpe conmocionador | Melee | 15 | 60° | 2,5 | 15 s | −25 | **aturde 4 s** (con RD) |
| Patada | Melee | 8 | 60° | 2,5 | 10 s | +5 | **interrumpe** |
| Salto | Leap (punto) | 30 en 2,5 m | — | 8 | 12 s | +10 | salto en arco 0,45 s |
| Grito de guerra | WarCry | 0 | radio 6 | — | 20 s | +25 | ralentiza 40 % 4 s |

Definitivas:

| Definitiva | Recarga | Qué hace |
|---|---|---|
| Furia | 60 s | 8 s con daño ×1,3 y 25 % de robo de vida (la velocidad de ataque queda para cuando haya animaciones) |
| Golpe del Conquistador | 45 s | Necesita objetivo y ≥30 de Rabia. 1 s de carga inmóvil, gasta toda la Rabia, daño 80 + 2 por Rabia gastada. Si el objetivo sale del alcance (3 m ×1,25), muere, o al jugador lo aturden/interrumpen, **falla y la recarga corre igual** |

Barra inicial: Golpe · Tajo giratorio, Golpe demoledor, Desgarrar, Golpe
conmocionador, Patada, Salto · Golpe del Conquistador. Ejecutar, Ráfaga,
Hendidura, Grito y Furia están creadas: se cambian arrastrando el asset a la
ranura en el Inspector del `AbilityCaster` del `Player` (o en
`CreateDevastadorKit` del constructor de escena).

Colores de botón: blanco = genera Rabia, naranja = gasta, amarillo = control,
rojo = definitiva.

## Pasos en Unity

Nada manual; **Umbralis → 1** regenera escena y assets (si un asset ya existe,
solo actualiza sus valores).

## Qué comprobar en el móvil

Combo del diseño
- Golpe conmocionador sobre un Acechador → cubo amarillo, se queda quieto 4 s.
  Con ≥30 de Rabia, Golpe del Conquistador: barra de carga 1 s, número grande.
- El mismo combo otra vez seguida: el aturdimiento dura 2 s, luego 1 s, luego
  nada (inmune). Tras 15 s vuelve a durar 4.
- Golpe del Conquistador **sin** aturdir antes: si el enemigo se mueve más de
  ~4 m en el segundo de carga, falla (cubo pequeño) y la Rabia y la recarga se
  pierden. ¿Se siente justo?

Interrupción
- Cuando el Acechador está anunciando (círculo rojo llenándose), Patada: el
  círculo desaparece y no pega.

Resto
- Desgarrar deja números pequeños cada segundo durante 6 s.
- Salto: mantener y arrastrar muestra el disco; soltar salta hasta él y daña al
  caer. Toque rápido salta delante del objetivo.
- Los botones apagados (naranja oscuro) se encienden al tener Rabia.
- ¿Los nombres se leen en los botones? El tamaño de letra se ajusta solo.
