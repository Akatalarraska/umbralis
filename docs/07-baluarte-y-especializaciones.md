# Fase 2 — Baluarte y cambio de especialización

Primeras dos tareas de la Fase 2. APK en `client/Builds/Umbralis.apk`.

## Especializaciones por datos

`Scripts/Abilities/SpecializationDefinition.cs` (assets en
`Data/Especializaciones/`): ataque básico, 10 habilidades, 2 definitivas,
pasiva y qué 6 + 1 van en la barra por defecto. `PassiveDefinition` es un
asset que sabe añadir su componente al personaje y quitarlo.

`Scripts/Player/SpecializationSwitcher.cs` en el jugador tiene las dos
(Devastador, Baluarte). Cambiar: botón **"Especialización"** arriba a la
derecha (muestra la activa). **Solo fuera de combate**: en combate el botón se
apaga (lo decide `ClassResource.InCombat`: 5 s sin dar ni recibir daño). Al
cambiar se sustituye la barra, se ponen las recargas a cero, se quita la pasiva
anterior y se pone la nueva, y el HUD carga el **perfil de disposición de esa
especialización** (cada una guarda la suya en PlayerPrefs).

## Sistema de modificadores de daño

`IDamageModifier` en `StatusEffects`: cualquier cosa que cambie el daño que un
personaje hace o recibe (pasivas, potenciaciones defensivas, redirecciones).
`Health.TakeDamage` pasa el daño entrante por ellos y `Health.DealDamage` el
saliente. Los temporales caen al morir; los permanentes (pasivas) no.

## Amenaza y provocación (IA)

`EnemyBrain` lleva ahora una tabla de amenaza: cada punto de daño recibido
suma amenaza a quien lo hizo; las habilidades de tanque suman extra; una
provocación fuerza el objetivo 4 s y deja al provocador el primero. El enemigo
cambia de objetivo solo si otro le supera en un 20 % (`switchThreshold`).
Fuera de combate se olvida todo.

Para probarlo hay un **Aliado** (cápsula verde, 300 de vida, en el fondo) que
pincha 6 de daño por segundo a los enemigos a 4 m: si llevas un Acechador
hasta él, acabará atacando al aliado y hay que recuperarlo.

## Pasivas

- **Muralla** (Baluarte): 30 % menos de daño **de frente** (el atacante está
  delante); cada golpe bloqueado así da una carga (máx. 5) que sube un 10 %
  por carga el siguiente golpe; **Aplastar** las consume con un 20 % extra por
  carga. Las cargas se ven bajo la barra de Rabia ("Muralla 3 / 5").
- **Sed de batalla** (Devastador): por encima de 50 de Rabia el ataque básico
  hace un 25 % más. Se ve "Sed de batalla ACTIVA" bajo la Rabia.

## Kit del Baluarte (`Data/Baluarte/`)

| Habilidad | Tipo | Valores | Efecto |
|---|---|---|---|
| Golpe de escudo | Melee | 15 dmg, 60°, 2,5 m, 8 s, +5 | interrumpe |
| Tajo de barrido | Melee | 20 dmg, 120°, 3 m, 4 s, +10 | +40 amenaza por objetivo |
| Lanzar escudo | ShieldThrow | 20 dmg ×3 rebotes (6 m entre ellos), 12 m, 10 s, +5 | atrae 2 m, +30 amenaza |
| Aplastar | Melee | 45 dmg, 60°, 2,5 m, 8 s, −25 | +20 % por carga de Muralla |
| Represalia | SelfBuff | 6 s, 20 s, −20 | devuelve el 50 % del daño recibido |
| Carga con escudo | Dash | 15 dmg, 8 m, 12 s, +10 | aturde 1,5 s (con RD) |
| Muro de escudo | SelfBuff | 6 s, 25 s | 60 % menos daño de frente |
| Grito de desafío | Taunt | radio 8, 20 s, +15 | provoca 4 s a todos, +60 amenaza |
| Provocar | Taunt | 15 m, 10 s, +10 | provoca 4 s al objetivo, +50 amenaza |
| Interceptar | Intercept | 15 m, 20 s, +5 | salta al aliado más cercano; 6 s encajando el 50 % de su daño |

Definitivas: **Última Posición** (4 s sin poder bajar de 1 de vida, 90 s) y
**Estandarte de guerra** (8 s, 30 % menos daño para los aliados a 8 m, 60 s).

Barra inicial: Golpe · Golpe de escudo, Tajo de barrido, Lanzar escudo,
Aplastar, Carga con escudo, Provocar · Última Posición.

Tipos nuevos: `SelfBuffAbility`, `TauntAbility`, `ShieldThrowAbility`,
`InterceptAbility`; `MeleeAbility` gana `threat` y `bonusPerBulwarkCharge`;
`DashAbility` gana `stunDuration`.

## Qué comprobar en el móvil

Cambio
- "Especialización" cambia Devastador ↔ Baluarte fuera de combate; en combate
  el botón está gris. La barra cambia de nombres y colores.
- Mueve un botón en Baluarte, cambia a Devastador: cada una recuerda su sitio.

Muralla
- De frente a un Acechador, sus golpes hacen ~10 en vez de 15 y "Muralla" sube
  de 1 en 1; de espaldas hacen 15 y no sube.
- Con 5 cargas, Aplastar hace el doble (45 → ~90).

Amenaza
- Lleva un Acechador al Aliado verde y espera: cuando el aliado le pegue unas
  veces, el Acechador se gira hacia él. Provocar lo trae de vuelta 4 s; después
  ¿se queda contigo? (debería, por la amenaza extra).
- Interceptar: saltas junto al aliado y durante 6 s parte de sus golpes salen
  en rojo sobre ti.

Resto
- Lanzar escudo con 2–3 enemigos juntos: rebota y los acerca.
- Golpe de escudo durante el círculo rojo lo cancela.
- Última Posición con poca vida: los golpes te dejan en 1 y no mueres 4 s.
