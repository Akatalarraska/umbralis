# Fase 1 — Movimiento y cámara táctil

Qué hay que hacer a mano en Unity para probar el controlador en el móvil.

## 1. Crear el proyecto Unity dentro de `client/`

El repositorio ya contiene `client/Assets/_Umbralis/` con los scripts, pero no
el resto del proyecto (ProjectSettings, Packages…), que lo genera Unity Hub.

1. Unity Hub → **New project** → versión **6000.x** → plantilla **Universal 3D**
   (también sirve "Mobile 3D"; ambas traen URP).
2. Nombre: `Umbralis`. Ubicación: `C:\Users\Robert\Desktop\umbralis`.
   Se creará `C:\Users\Robert\Desktop\umbralis\Umbralis\`.
3. Cierra Unity. Mueve **todo el contenido** de `Umbralis\` dentro de `client\`
   (Windows fusiona la carpeta `Assets`). Borra la carpeta `Umbralis\` vacía.
4. En Unity Hub → **Add project from disk** → selecciona `client\`.

## 2. Montar la escena (automático)

Con el proyecto abierto, menú **Umbralis → 1. Crear escena de prueba de combate**.
Crea y guarda `Assets/_Umbralis/Scenes/CombatPrototype.unity` con suelo,
obstáculos, jugador (cápsula azul con marcador amarillo delante), cámara, HUD
con joystick y zona de cámara, EventSystem y GameBootstrap.

Pulsa Play: en el editor el ratón simula el dedo (mitad izquierda = joystick,
mitad derecha = cámara). Para probar dos dedos usa Window → General →
**Device Simulator** o directamente el móvil.

## 3. Ajustes de Android

1. Menú **Umbralis → 2. Aplicar ajustes de Android**: fija nombre, identificador,
   orientación LandscapeLeft y cambia la plataforma a Android.
2. Comprueba en **Edit → Project Settings → Player → Android → Resolution and
   Presentation** que *Default Orientation* es **Landscape Left**.
3. En **Player → Other Settings → Active Input Handling** vale tanto
   *Input System Package (New)* como *Both*; los controles usan el EventSystem
   de UI y funcionan con ambos.

## 4. Compilar e instalar

1. Activa *Opciones de desarrollador → Depuración USB* en el móvil.
2. **File → Build Profiles → Android → Build And Run** con el móvil conectado.

## Qué comprobar en el móvil

- Tocar en la mitad izquierda hace aparecer el joystick bajo el dedo.
- El personaje se mueve relativo a la cámara y gira hacia donde anda.
- Arrastrar en la mitad derecha orbita la cámara (pulgar derecho) mientras
  el pulgar izquierdo sigue moviendo al personaje.
- La cámara se acerca al pasar detrás de un cubo en vez de atravesarlo.

Ajustes de sensación en el Inspector: `PlayerMovement.moveSpeed`,
`FloatingJoystick.radius / deadZone`, `ThirdPersonCamera.degreesPerScreenWidth`.
