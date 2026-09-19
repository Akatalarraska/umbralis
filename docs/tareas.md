# Tareas

Orden de trabajo. No pases de fase sin probar en un móvil real.

## Fase 0 — Repositorio

- [x] `.gitignore` oficial de Unity en la raíz
- [x] `.gitattributes` con Git LFS para `.fbx .png .jpg .wav .anim .psd`
- [x] Proyecto Unity (URP) en `client/`, orientación horizontal bloqueada
- [x] Compilar un APK vacío e instalarlo en el móvil, para tener el ciclo montado

## Fase 1 — Prueba de combate (fase actual)

El objetivo es saber si el combate es divertido con los dedos. Cada tarea se
prueba en el móvil antes de seguir.

- [x] Controlador de personaje en tercera persona
- [x] Joystick virtual flotante en la mitad izquierda de la pantalla
- [ ] Cámara controlada arrastrando en la mitad derecha, con pellizco para zoom
      *(hecho el arrastre; falta el pellizco)*
- [x] Esquiva (Embestida) con recarga
- [x] Sistema de habilidades por datos: cada habilidad es un ScriptableObject con
      nombre, daño, coste de recurso, recarga, alcance y tipo de apuntado
- [x] Recurso Rabia 0–100: sube al golpear y al recibir daño, baja fuera de combate
- [x] Barra de combate: ataque básico + 6 habilidades + definitiva + esquiva
- [x] Apuntado híbrido: toque rápido al objetivo, mantener y arrastrar para apuntar
- [x] Selección de objetivo: tocar al enemigo y botón de cambio
      *(falta la prioridad automática configurable: ahora siempre "más cercano")*
- [x] Enemigo básico con vida, IA simple y ataque telegrafiado con zona en el suelo
      *(falta el aviso sonoro; de momento solo visual)*
- [x] Rendimientos decrecientes en aturdimientos
- [x] Números de daño flotantes y barras de vida sobre los personajes
- [x] Las 10 habilidades del Devastador con sus recargas
- [x] Definitivas: Furia y Golpe del Conquistador (carga de 1 s, se puede fallar)
      *(Furia sin velocidad de ataque hasta que haya animaciones)*
- [ ] Muñeco de entrenamiento con medidor de daño por segundo y desglose
      *(hecho `TrainingDummy` con parpadeo, empujón y reaparición; falta el medidor)*

### Criterios para dar la fase por buena

- Se juega cómodo a dos manos en horizontal, sin fallos al pulsar botones.
- Hay sensación de impacto al golpear.
- El combo aturdir + Golpe del Conquistador se siente técnico y satisfactorio.
- Apetece seguir pegando al muñeco cinco minutos sin ningún motivo.

## Fase 2 — Segunda especialización y roles

- [ ] Baluarte (tanque) con su pasiva Muralla y sus 10 habilidades
- [ ] Cambio de especialización gratis fuera de combate, con barra propia
- [ ] Templario/Juramentado, para tener la clase espejo y comprobar que los
      números son idénticos y solo cambia lo visual

## Fase 3 — Prueba de red

- [ ] Elegir solución de red y arquitectura de servidor
- [ ] Dos jugadores en la misma instancia, con el servidor validando daño y vida
- [ ] Medir latencia y comportamiento con conexión móvil
- [ ] Reconexión sin penalización en menos de 2 minutos

## Fase 4 — Bucle completo

- [ ] Experiencia y niveles 1 a 10
- [ ] Equipo básico con atributos y huecos
- [ ] Botín con tablas fijas por enemigo
- [ ] Inventario mínimo y comparación de piezas

## Fase 5 — Prototipo vertical

- [ ] Una zona pequeña de mundo abierto con misiones
- [ ] Una mazmorra de 3 jugadores sin rol de sanador
- [ ] Un battleground 5v5
- [ ] Pings y buscador de grupo simple
- [ ] Pruebas con jugadores reales y medición de retención

## Pendientes de diseño

Se resuelven cuando toque, no bloquean el prototipo.

- [ ] Nombres de zonas, de la Quiebra y de los personajes clave
- [ ] Mecánicas concretas de cada jefe
- [ ] Números de equilibrio: daño, vida, curvas, precios, umbrales de división
- [ ] Comprobar y registrar la marca Umbralis (EUIPO, OEPM, Google Play)
- [ ] Reservar dominio y perfiles de redes
