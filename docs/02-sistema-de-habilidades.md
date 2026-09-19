# Fase 2 — Sistema de habilidades

Cuatro habilidades de prueba, botones táctiles al estilo MOBA y muñecos de
entrenamiento para sentir el impacto. Todo se monta con **Umbralis → 1. Crear
escena de prueba de combate** (regenera la escena; conserva los assets de
`Data/` y `Materials/` si ya existen, solo actualiza sus valores).

## Controles (pulgar derecho)

| Gesto sobre un botón            | Resultado                                                        |
|---------------------------------|------------------------------------------------------------------|
| Toque corto                     | Lanza al instante apuntando al enemigo vivo más cercano (o al frente). |
| Mantener y arrastrar            | Modo apuntado: franja (dirección) o disco (punto) en el suelo. Soltar lanza. |
| Arrastrar de vuelta al botón    | El botón se pone rojo; soltar cancela.                            |
| Botón sombreado                 | En enfriamiento; el toque se ignora.                              |

El arrastre es relativo a la cámara (arriba = alejarse de la cámara), igual que
el joystick. Mientras se apunta, el joystick izquierdo sigue moviendo al
personaje. Al lanzar, el personaje encara la dirección durante
`faceLockDuration` segundos.

## Habilidades incluidas (`Assets/_Umbralis/Data/`)

| Asset          | Tipo                 | Apuntado  | Valores iniciales                          |
|----------------|----------------------|-----------|--------------------------------------------|
| `Melee`        | `MeleeAbility`       | Dirección | 20 daño, cono 100°, alcance 2.5, CD 0.5 s  |
| `Projectile`   | `ProjectileAbility`  | Dirección | 30 daño, 18 m/s, alcance 14, CD 2 s        |
| `Dash`         | `DashAbility`        | Dirección | 15 daño, 5 m en 0.2 s, CD 4 s (tipo "Carga"; no va en la barra) |
| `AreaBlast`    | `AreaBlastAbility`   | Punto     | 40 daño, radio 2.5, alcance 8, CD 6 s      |

Cada asset es una muestra de un **tipo** de habilidad; las habilidades reales de
cada clase serán assets de estos tipos (y de los que se añadan: sangrado,
aturdir, interrumpir…). La esquiva no es una habilidad: ver `03-esquiva.md`.

Todos los números se editan en el Inspector del asset y se aplican sin tocar
código. Nuevas habilidades: heredar de `AbilityDefinition`, implementar
`Execute(in AbilityContext)` y crear el asset con
**Create → Umbralis → Habilidades**. Luego asignarla a una ranura del
`AbilityCaster` del jugador (o en `CreateAbilityAssets` del constructor de escena).

## Piezas de código

- `Scripts/Abilities/AbilityDefinition.cs` — ScriptableObject base: nombre, color,
  enfriamiento, alcance, modo de apuntado, material de efecto. Utilidades
  `SpawnFx` y `DamageInSphere` (con cono opcional).
- `Scripts/Abilities/AbilityCaster.cs` — en el jugador: ranuras, enfriamientos,
  `TryCastAuto` (auto-apuntado) y `TryCast` (dirección/punto explícitos).
- `Scripts/TouchControls/AbilityButton.cs` — el gesto del botón. Parámetros
  `aimThreshold`, `cancelRadius`, `maxDrag` (unidades del Canvas 1920×1080).
- `Scripts/Combat/Health.cs` — vida, equipo (`Player`/`Enemy`), eventos y registro
  estático `Health.All` para buscar objetivos sin capas ni etiquetas.
- `Scripts/Combat/TrainingDummy.cs` — parpadeo, empujón y reaparición a los 2 s.
- `Scripts/Combat/HealthBar.cs`, `AimIndicator.cs`, `Projectile.cs` — visuales y
  proyectil por SphereCast (no atraviesa objetivos por velocidad).
- `PlayerMovement.Dash` / `LockFacing` — usados por la embestida y por cualquier
  lanzamiento.

## Qué comprobar en el móvil

- El toque corto en "Golpe" gira al personaje hacia el muñeco más cercano y le
  quita vida (barra verde sobre la cabeza); el muñeco parpadea y retrocede.
- Mantener "Disparo" y arrastrar muestra la franja amarilla; soltar dispara una
  esfera naranja que se para en cubos y muñecos.
- "Estallido" muestra un disco en el punto; el disco no pasa del alcance (8 m).
- Volver con el pulgar al botón lo pone rojo y al soltar no pasa nada.
- Los muñecos muertos desaparecen y vuelven a los 2 s con la vida completa.
- ¿Los botones caen bien bajo el pulgar derecho sin estorbar al giro de cámara?
  Sus posiciones están en `CreateAbilityBar` (esquina inferior derecha).
