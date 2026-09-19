# Umbralis — contexto del proyecto

Lee este archivo antes de escribir código. El diseño completo está en `docs/gdd.md`
y las tareas en `docs/tareas.md`.

## Qué es

Umbralis es un RPG online de fantasía para **móvil Android**, tipo MMO-lite:
mundo abierto individual (online, validado por servidor, compartible con 2–5
amigos) y contenido multijugador instanciado (mazmorras de 3–5, battlegrounds de
5v5 a 20v20, bandas de 10). Dos facciones enfrentadas: La Llama Blanca y El Pacto
Oscuro.

Fase actual: **prototipo de combate en local**. Sin red, sin servidor, sin arte
definitivo, sin menús. El objetivo es responder a una sola pregunta: ¿el combate
resulta divertido con los dedos en una pantalla de móvil?

## Reglas de trabajo

- El motor es **Unity (URP)**, el proyecto vive en `client/`, el código es C#.
- Escribe **comentarios y nombres de sistemas en español**; los nombres de clases
  C# en inglés solo si es lo idiomático del motor.
- **Un script por responsabilidad**, en carpetas claras dentro de `Assets/Scripts/`.
- Lo que no se pueda hacer desde código (escena, prefabs, importar assets) se
  explica al final de cada tarea como **pasos manuales en el editor de Unity**.
- Nada de arte: primitivas (cápsulas, cubos) y materiales planos.
- Todo debe funcionar en una compilación Android real, **orientación horizontal
  bloqueada**.
- Avanza por tareas pequeñas y comprobables. No implementes varias tareas de
  golpe: hay que probar cada una en el móvil antes de seguir.

## Decisiones de diseño que afectan al código

### Combate híbrido
- Habilidades **dirigidas**: van al objetivo seleccionado; si no hay, al enemigo
  más cercano en alcance.
- Habilidades **direccionales** (líneas, conos, zonas): se apuntan manteniendo y
  arrastrando el botón. Un toque rápido las lanza hacia el objetivo actual.
- Selección: tocar al enemigo, botón para cambiar de objetivo, y prioridad
  automática configurable (más cercano / menos vida / jugadores antes que NPC).

### Barra de combate
- Ataque básico (botón grande) + **6 habilidades** + 1 definitiva + esquiva.
- Cada especialización tiene 10 habilidades disponibles y el jugador elige 6.
- Botones extra por clase: alimentar bestia (Animalista), resurrección fuera de
  combate (sanadores).
- Todos los botones deben poder moverse y escalarse.

### Recursos por clase
Cada clase tiene un recurso distinto, siempre legible en una barra o en cargas:
Rabia (Conquistador), Concentración (Montero), Maná (Hechicero), 6 Reactivos
(Alquimista), Fervor/Tributo (Templario/Juramentado), Devoción/Esencia
(Clérigo/Espiritista).

### Clase del prototipo: Conquistador
- Armadura pesada. Recurso **Rabia** 0–100: sube al golpear y al recibir daño,
  baja poco a poco fuera de combate; las habilidades fuertes la consumen.
- Especialización **Devastador** (daño cuerpo a cuerpo), la primera a implementar:
  Tajo giratorio, Golpe demoledor, Ejecutar, Desgarrar, Ráfaga de golpes,
  Hendidura, Golpe conmocionador (aturde 4 s), Patada (interrumpe), Salto,
  Grito de guerra. Definitivas: Furia o Golpe del Conquistador.
- Especialización **Baluarte** (tanque), después.
- Esquiva: **Embestida**, avance corto hacia delante.

### Reglas globales de combate
- Mismos valores en todos los modos: no hay estadísticas distintas en PvP.
- **Rendimientos decrecientes** en todos los controles: un mismo objetivo recibe
  aturdimientos cada vez más cortos hasta quedar inmune un tiempo.
- Todo ataque enemigo importante se **telegrafía**: zona en el suelo, sonido y al
  menos 1,5 s de aviso antes del impacto.
- Los efectos de los enemigos son siempre visibles; los de aliados, reducibles.
  Bajar la calidad gráfica nunca oculta información ni da ventaja.
- Todo aviso sonoro lleva equivalente visual: el juego debe poder jugarse en
  silencio.

### Rendimiento
Objetivo: 60 FPS en gama media y 30 estables en gama baja. Presets gráficos
(bajo/medio/alto), límite de FPS y modo de ahorro de batería. Hay que pensar en
que algún día habrá 40 personajes en pantalla.

## Estructura del repositorio

```
client/    proyecto Unity (Umbralis)
docs/      diseño y tareas
server/    vacío por ahora; se llenará en la fase de red
```
