# Umbralis

Prototipo de combate para móvil (Android) hecho en Unity 6 con URP.

Objetivo de esta fase: comprobar si el combate resulta divertido con los dedos
sobre una pantalla táctil. Sin red, sin menús, sin progresión y sin arte
definitivo (los personajes son cápsulas y cubos).

## Estructura del repositorio

| Carpeta   | Contenido                                                                 |
|-----------|---------------------------------------------------------------------------|
| `client/` | Proyecto Unity `Umbralis` (Unity 6, URP). El código está en `Assets/_Umbralis/`. |
| `docs/`   | Documentos de diseño, notas de pruebas y decisiones.                      |
| `server/` | Reservado para el backend. Vacío en esta fase (no hay red).               |

## Dentro de `client/Assets/_Umbralis/`

- `Scripts/Core/` — arranque y ajustes globales (orientación, FPS).
- `Scripts/TouchControls/` — joystick virtual flotante y zona de cámara táctil.
- `Scripts/Player/` — movimiento en tercera persona y cámara orbital.
- `Editor/` — utilidades de editor (menú `Umbralis` para montar la escena de prueba).
- `Scenes/`, `Materials/`, `Prefabs/`, `Data/` — assets generados o creados en el editor.

## Requisitos

- Unity 6 (6000.x) con el módulo de Android.
- Git LFS instalado (`git lfs install`) para los binarios listados en `.gitattributes`.
