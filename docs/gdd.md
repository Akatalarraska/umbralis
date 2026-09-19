# Umbralis — Documento de diseño

## Visión y alcance

Umbralis es un RPG online de fantasía para móvil con mundo abierto individual y contenido multijugador instanciado: mazmorras de 3 a 5 jugadores, battlegrounds de 5v5 a 20v20 y bandas de 10. No es un MMO de mundo compartido, sino un MMO-lite, lo que elimina la necesidad de simular un mundo persistente para miles de jugadores.

| Decisión | Detalle |
| --- | --- |
| Género | RPG online de fantasía (MMO-lite) |
| Mundo abierto | Individual y online, compartible con 2–5 amigos por invitación |
| Contenido online | Ciudades sociales, mazmorras, battlegrounds, Campañas y bandas |
| Progreso | Validado por servidor; mismas estadísticas en todos los modos |
| Plataforma | Android primero; iOS más adelante o con financiación |
| Orientación | Horizontal, sin modo vertical |
| Desarrollo | Por cuenta propia: primero el diseño completo, decidir después |

### Pilares de diseño

1. Legible y fluido en cualquier móvil: cada decisión de arte, efectos e interfaz se filtra por esta pregunta.
2. Meritocracia: el mejor equipo y los mejores rangos se ganan jugando, nunca pagando.
3. Sesiones cortas: mazmorras de 15–25 minutos, partidas de PvP por debajo de 20, bandas divididas en alas.
4. Rivalidad real entre dos facciones, con una amenaza común que obliga a colaborar a regañadientes.
5. Ninguna facción es la buena: las dos tienen razones legítimas y cosas que ocultar.

### Descripción pública

"RPG online de fantasía con mundo abierto, mazmorras cooperativas y battlegrounds PvP". Se evita prometer "MMO de mundo abierto" para no generar la expectativa de ver cientos de jugadores por el mapa.

## Mundo y narrativa

Umbralis es el mundo; Vastia, el continente donde ocurre el juego. Todo el lore gira sobre una frontera: el Umbral, que separa a los vivos de los muertos.

| Regla | Qué significa |
| --- | --- |
| El Umbral | Al morir, el alma cruza una frontera invisible hacia el otro lado |
| El Descanso | Al otro lado las almas descansan; para la Llama Blanca es sagrado |
| El Retorno | Un alma puede ser llamada de vuelta, pero cada retorno la desgasta |
| El Olvido | Amenaza que devora almas a ambos lados: ni descansan ni pueden volver. Sus criaturas son los Borrados |

### Cronología

1. Era del Descanso: todos los pueblos comparten las tierras y nadie cruza el Umbral.
2. La Quiebra: un cataclismo rompe parte del Umbral y deja tierras grises y frías.
3. Era del Pacto: los pueblos atrapados pactan con sus muertos para sobrevivir.
4. Guerra del Retorno: el Pacto vuelve a reclamar lo perdido; la Llama Blanca crea su Inquisición.
5. Presente: el Olvido entra por las grietas y obliga a colaborar sin apagar la guerra.

El secreto: la Quiebra la provocaron los antepasados de la Llama Blanca al intentar sellar el Umbral para siempre.

### Geografía

Vastia tiene forma de A. Cada facción parte de una esquina inferior, separadas por un mar interior; ambas ramas desembocan en el mismo punto y desde ahí el camino es común y equidistante hasta la cima.

| Tramo | Contenido |
| --- | --- |
| Esquina izquierda | Pacto Oscuro: capital Osvar, introducciones por raza (1–3), zonas 4–9 y 10–15 |
| Esquina derecha | La Llama Blanca: capital Valdor, introducciones por raza (1–3), zonas 4–9 y 10–15 |
| Punto de unión | Zona 16–20, simétrica, con campamento de cada facción y el frente en medio |
| Camino común | Zonas 21–25, 26–30, 31–35, 36–40 y 41–45 |
| Cima | La Quiebra: zonas 46–50, mazmorras de nivel 50 y la banda |

Regla de equidistancia: ninguna facción tiene nunca un viaje más largo ni una zona más difícil más cerca. La paleta se desatura al ascender: el mundo se vuelve más sombrío conforme el jugador se acerca a la Quiebra.

### Narrativa

| Capa | Contenido |
| --- | --- |
| Campaña de facción (1–15) | Cada bando cuenta su versión del conflicto |
| Campaña de guerra (16–45) | Misma zona, misiones distintas por facción; aparecen las Brechas |
| Campaña del Olvido (46–50) | Ambas facciones llegan a la Quiebra y descubren la verdad |
| Temporadas | Cada temporada avanza la historia del Olvido |

Personajes clave definidos por rol, pendientes de nombre: Soberana de la Llama, Gran Inquisidor, Guardiana del Secreto, Caudillo del Pacto, el Primer Retornado, el Mercader de Almas y el Heraldo del Olvido.

En móvil: diálogos de tres líneas como máximo y saltables, resumen de una línea al saltar, cinemáticas cortas con el motor, voces solo en personajes clave, códice de lore y recapitulación al volver tras tiempo sin jugar.

## Facciones y razas

Dos facciones enfrentadas con tres razas cada una. La Llama Blanca cree que los muertos deben descansar; el Pacto Oscuro convive con ellos y los llama de vuelta.

| Facción | Capital | Razas | Idea |
| --- | --- | --- | --- |
| La Llama Blanca | Valdor | Valdrenos (humanos), Ithrani (elfos), Grundar (enanos) | Perturbar a los muertos es la mayor abominación |
| El Pacto Oscuro | Osvar | Vorkrath (orcos), No-muertos, Grisk (goblins) | Sobrevivieron pactando con la muerte, y el pacto cobra un precio |

### Razas

| Raza | Tamaño | Relación con la muerte |
| --- | --- | --- |
| Valdrenos | Medio | Núcleo de la fe y de la Inquisición |
| Ithrani | Medio, algo más altos | Recuerdan el cataclismo y guardan el secreto |
| Grundar | Pequeño | Sellan a sus muertos en piedra con runas |
| Vorkrath | Algo más altos que los Ithrani | Luchar junto a los ancestros es un honor |
| No-muertos | Medio | Son el pacto en carne propia |
| Grisk | Pequeño | Se unieron por negocio: comerciar con almas es rentable |

Ambos sexos en las seis razas (12 modelos base), esqueletos compartidos por tamaño y personalización básica en el lanzamiento. Sin pasivas raciales: ninguna raza da ventaja en combate, la identidad viene del aspecto, la zona inicial, la historia y los emotes.

### Reglas entre facciones

| Ámbito | Regla |
| --- | --- |
| PvP | Siempre facción contra facción |
| Mazmorras y bandas | Facciones mezcladas, con chat de grupo |
| Comunicación | Ninguna con el bando rival, salvo dentro del grupo o por susurro entre amigos confirmados |
| Comercio directo | Solo dentro de la misma facción |
| Gran Mercado | Común, con un 10 % extra al comprar a la otra facción |
| Desequilibrio de población | La facción con menos jugadores recibe más Óbolos y experiencia |
| Cambio de facción | Solo hacia la facción minoritaria |

## Clases y combate

Seis clases con dos especializaciones cada una: cuatro compartidas y dos espejo, idénticas en números entre facciones y distintas solo en nombre, animaciones, efectos y sonido.

| Clase | Armadura | Recurso | Especialización 1 | Especialización 2 |
| --- | --- | --- | --- | --- |
| Conquistador | Pesada | Rabia | Baluarte (tanque) | Devastador (daño cuerpo a cuerpo) |
| Montero | Media | Concentración | Tirador (daño a distancia) | Animalista (daño con bestia) |
| Hechicero | Ligera | Maná | Elementalista (daño en área) | Tejedor (daño y control) |
| Alquimista | Media | 6 Reactivos | Elixirista (sanador preventivo) | Artificiero (daño a distancia) |
| Templario / Juramentado | Pesada | Fervor / Tributo | Guardallama / Guardatumbas (tanque) | Inquisidor / Segador (daño) |
| Clérigo / Espiritista | Ligera | Devoción / Esencia | Ungido / Guía de Almas (sanador reactivo) | Heraldo / Poseído (daño mágico) |

### Razas y clases

| Clase | Valdrenos | Ithrani | Grundar | Vorkrath | No-muertos | Grisk |
| --- | --- | --- | --- | --- | --- | --- |
| Conquistador | Sí | — | Sí | Sí | Sí | — |
| Montero | Sí | Sí | Sí | Sí | — | Sí |
| Hechicero | — | Sí | Sí | — | Sí | Sí |
| Alquimista | Sí | — | Sí | — | Sí | Sí |
| Templario | Sí | Sí | Sí | — | — | — |
| Clérigo | Sí | Sí | — | — | — | — |
| Juramentado | — | — | — | Sí | Sí | Sí |
| Espiritista | — | — | — | Sí | Sí | — |

Toda raza puede jugar los tres roles. Cada facción tiene dos clases de tanque, dos de sanador y opción de daño en todas.

### Reglas comunes

| Regla | Detalle |
| --- | --- |
| Barra | Ataque básico, 6 habilidades, definitiva y esquiva |
| Habilidades | 10 disponibles por especialización, se eligen 6 |
| Definitivas | 2 por especialización, se elige 1 |
| Botones extra | Alimentar bestia (Animalista) y resurrección fuera de combate (sanadores) |
| Tanques y sanadores | Hacen daño relevante: 50–60 % y 30–40 % de una especialización de daño |
| Controles | Rendimientos decrecientes en aturdimientos y efectos de control |
| Cambio de especialización | Gratis fuera de combate, con barra, talentos y conjunto propios |
| Dificultad de clase | No se indica en ninguna pantalla |

### Combate híbrido

| Tipo de habilidad | Comportamiento |
| --- | --- |
| Dirigida | Va al objetivo seleccionado o, si no hay, al enemigo más cercano |
| Direccional | Se apunta arrastrando el botón; un toque rápido la lanza hacia el objetivo |
| Curación | Tocando el retrato del aliado o al seleccionado |

La selección se hace tocando al enemigo, con un botón para cambiar de objetivo y prioridad automática configurable (más cercano, menos vida o jugadores antes que NPC).

### Conquistador

Guerrero de primera línea que se hace más peligroso cuanto más dura el combate. Armadura pesada. Razas: Valdrenos, Grundar, Vorkrath y No-muertos.

Recurso **Rabia** (0–100): se genera al golpear y al recibir daño, baja poco a poco fuera de combate; las habilidades básicas la generan y las fuertes la gastan. Esquiva: **Embestida**, avance corto hacia delante.

Armas: arma a una mano con escudo (Baluarte); arma a dos manos o dos armas a una mano, con el mismo daño total y distinto ritmo (Devastador).

**Baluarte (tanque).** Pasiva Muralla: recibe menos daño de frente y cada ataque bloqueado aumenta el daño de su siguiente golpe (hasta 5 cargas).

| Habilidad | Efecto |
| --- | --- |
| Golpe de escudo | Daño que interrumpe el lanzamiento enemigo |
| Tajo de barrido | Corte frontal que genera Rabia y amenaza |
| Lanzar escudo | Rebota entre 3 enemigos, daña y los atrae |
| Aplastar | Gasta Rabia; más daño con cargas de Muralla |
| Represalia | Durante unos segundos, cada golpe recibido devuelve daño |
| Carga con escudo | Se lanza hacia un enemigo y lo aturde brevemente |
| Muro de escudo | Reduce mucho el daño frontal |
| Grito de desafío | Obliga a los enemigos cercanos a atacarle |
| Provocar | Obliga a un enemigo a atacarle, a distancia |
| Interceptar | Salta hacia un aliado y recibe parte de su daño |

Definitivas: Última Posición (4 s sin poder bajar de 1 punto de vida) o Estandarte de guerra (reduce el daño que recibe el grupo cercano).

**Devastador (daño cuerpo a cuerpo).** Pasiva Sed de batalla: por encima de 50 de Rabia, los ataques básicos hacen más daño.

| Habilidad | Efecto |
| --- | --- |
| Tajo giratorio | Daño a todos los enemigos alrededor |
| Golpe demoledor | Gasta Rabia para un golpe muy fuerte |
| Ejecutar | Mucho más daño a enemigos con poca vida |
| Desgarrar | Sangrado |
| Ráfaga de golpes | Tres golpes seguidos a un objetivo |
| Hendidura | Golpe en cono frontal |
| Golpe conmocionador | Aturde 4 segundos |
| Patada | Interrumpe el lanzamiento enemigo |
| Salto | Salta a una zona y hace daño al caer |
| Grito de guerra | Genera Rabia y ralentiza a los enemigos cercanos |

Definitivas: Furia (más velocidad de ataque, daño y robo de vida) o Golpe del Conquistador (1 s de carga inmóvil, gasta toda la Rabia, daño enorme; si el objetivo escapa o interrumpe, falla y entra en recarga igual).

### Montero

Cazador a distancia, armadura media, recurso **Concentración**, que se regenera más rápido estando quieto. Esquiva: Voltereta.

**Tirador.** Arco largo o mosquete. Pasiva Ojo de halcón: más daño cuanto más lejos está el objetivo. Tiro cargado, Ráfaga, Flecha perforante, Lluvia de flechas, Flecha de alquitrán, Señalar presa, Tiro a la rodilla, Flecha aturdidora, Disparo de corte, Disparo de retroceso. Definitivas: Tiro del Montero o Puesto de caza.

**Animalista.** Arco corto o trabuco, más cuchillo o daga. Pasiva Vínculo de caza: el Montero y su bestia hacen más daño si atacan al mismo objetivo. Orden: presa, Orden: derribar, Orden: proteger, Rugido de manada, Cepo, Red, Disparo gemelo, Degüello, A quemarropa, Camuflaje. Definitivas: Jauría o Furia primaria.

Botón fijo extra **Alimentar bestia**: instantáneo, cura a la mascota poco a poco, con anillo de vida junto a las habilidades. La bestia escala con el equipo del Montero. Las bestias domesticables son solo animales y criaturas naturales, se agrupan en familias (felinos, cánidos, plantígrados, rapaces, reptiles, cornudos) con patrón de daño propio y el mismo daño total; las épicas y legendarias solo dan prestigio visual.

### Hechicero

Armadura ligera, recurso **Maná**, que se recupera un poco con el ataque básico. Esquiva: Paso arcano, teletransporte corto.

**Elementalista.** Pasiva Confluencia: combinar dos elementos provoca reacciones (Incendio, Vapor, Magma, Lodo, Tormenta, Abrasión). Brasa, Surco ígneo, Marejada, Burbuja, Estacas de roca, Arenas movedizas, Coraza de piedra, Filo de viento, Vacío, Corriente ascendente. Definitivas: Gran Confluencia o Esencia primordial.

**Tejedor.** Pasiva Trama: cada sello sobre un objetivo aumenta el daño que le hace. Sello de ruina, Sello de desgaste, Desgarrar la trama, Hilo contagioso, Sifón de trama, Grilletes, Sello de silencio, Atadura, Glifo de repulsión, Espejismo. Definitivas: Círculo de sellos o Prisión rúnica.

### Alquimista

Armadura media, arma a una mano más foco (tomo, varita u orbe). Recurso **Reactivos**: 6 cargas visibles que se recuperan con el tiempo; el ataque básico acelera la recuperación. Esquiva: Retirada humeante.

**Elixirista (sanador preventivo).** Pasiva Destilado preventivo: la curación sobrante se convierte en escudo. Vial curativo, Tónico, Resina protectora, Bruma restauradora, Destilado concentrado, Antídoto, Estimulante, Frasco corrosivo, Humo narcótico, Ácido de alquimista (daña, reduce la curación recibida y ralentiza). Definitivas: Gran Elixir o Piedra filosofal.

**Artificiero (daño a distancia).** Pasiva Sobrecarga: más daño cuantas más cargas tenga. Bomba incendiaria, Frasco de ácido, Charco de brea, Metralla, Veneno volátil, Mezcla inestable, Destello cegador, Detonador, Humo cegador, Propulsión. Definitivas: Gran detonación o Lluvia de frascos.

### Templario (Llama Blanca) / Juramentado (Pacto Oscuro)

Armadura pesada, recurso **Fervor / Tributo**, que también se genera protegiendo. Esquiva: Paso ardiente / Paso de sombra.

**Guardallama / Guardatumbas (tanque).** Pasiva: parte del daño que hace le cura, más cuanto menos vida le queda. Golpe llameante, Estallido de brasas, Anillo de brasas, Condena, Reprimenda, Desafío, Égida, Llama interior, Voto de amparo, Avance sagrado (con sus equivalentes del Pacto). Definitivas: Llama inquebrantable / Legión ancestral, o Encarnación de la llama / Coloso de los caídos.

**Inquisidor / Segador (daño).** Pasiva Brasas del juicio / Almas errantes: algunos golpes dejan brasas o almas en el suelo que curan y generan recurso al pisarlas. Tajo ardiente, Ajusticiar / Siega, Marca del hereje / del condenado, Onda purificadora, Torbellino de brasas, Persecución, Cadenas de penitencia, Castigo cegador, Silencio del tribunal, Penitencia / Precio del pacto. Definitivas: Hoguera / Marea de difuntos, o Auto de fe / Juicio de la tumba.

### Clérigo (Llama Blanca) / Espiritista (Pacto Oscuro)

Armadura ligera, arma a una mano más foco. Recurso **Devoción / Esencia**, que se llena en lugar de gastarse: las habilidades básicas la generan y las potentes la consumen. Esquiva: Brillo fugaz / Velo fugaz.

**Ungido / Guía de Almas (sanador reactivo).** Pasiva Amparo / Tutela: se designa un aliado protegido que recibe un 30 % de cada curación hecha a otros. Luz reparadora, Bendición persistente, Plegaria de grupo, Gracia salvadora, Limpiar la mancha, Velo protector, Llama votiva, Rayo de fe, Resplandor cegador, Intercesión. Definitivas: Coro de la Llama / de ancestros, o Estado de gracia / Trance ancestral.

**Heraldo / Poseído (daño mágico).** Pasiva Coro / Legión menor: cada habilidad de daño invoca un eco (máximo 3) que repite una versión débil de la siguiente. Rayo de fe, Cirio ardiente, Haz del juicio, Estallido de fe, Clamor de ecos, Voz de la fe, Terror sagrado, Destierro, Mártir, Vuelo de ceniza. Definitivas: Descenso de la Llama / Llamada del abismo, o Iluminación / Posesión.

Todos los sanadores tienen además una acción fija fuera de la barra para **revivir al grupo** fuera de combate.

## Progresión

Nivel máximo 50, con unas 55–65 horas hasta el máximo en el primer personaje. La curva es muy rápida al principio y muy costosa al final.

| Niveles | Tiempo aproximado por nivel |
| --- | --- |
| 1–5 | 5–10 minutos |
| 6–20 | 30–45 minutos |
| 21–40 | 1–1,5 horas |
| 41–50 | 2–3 horas |

Cada nivel sube atributos y desbloquea algo: habilidades, contenido o sistemas. Ningún nivel queda vacío.

| Nivel | Desbloqueo |
| --- | --- |
| 1–4 | Ataque básico, esquiva y primeras habilidades del kit común |
| 3 | Llegada a la capital |
| 5 | Mazmorras de iniciación |
| 6 | Profesión de recolección |
| 8 | Coop por invitación y buscador de grupo |
| 9 | Gran Mercado y comercio |
| 10 | Especialización, battlegrounds y mazmorras con roles |
| 12 | Árbol de talentos |
| 13 | Primera montura |
| 15 | Zonas compartidas con la otra facción |
| 19 | Profesión de fabricación |
| 21 y 35 | Primera y segunda definitiva |
| 17, 25, 33, 41 | Habilidades 7 a 10 |
| 50 | Bandas, battlegrounds grandes, Campañas, renombre y mejora de equipo |

### Atributos

Suben solos con el nivel; el jugador no reparte puntos. La personalización viene del equipo.

| Principales | Efecto |
| --- | --- |
| Potencia | Daño y curación cuerpo a cuerpo |
| Destreza | Daño a distancia físico |
| Saber | Daño y curación mágicos |
| Vigor | Vida máxima |
| Armadura | Reduce el daño físico |
| Resguardo | Reduce el daño mágico |

| Secundarios | Efecto |
| --- | --- |
| Crítico | Probabilidad de golpe o curación crítica |
| Ferocidad | Daño y curación de los críticos |
| Presteza | Velocidad de ataque y recuperación de recurso |
| Enfoque | Reduce tiempos de recarga |
| Penetración | Ignora parte de armadura y resguardo |
| Firmeza | Reduce daño recibido y mejora el bloqueo |
| Gracia | Aumenta curación y escudos |
| Sustento | Robo de vida |
| Aplomo | Reduce la duración de los controles recibidos |
| Ligereza | Velocidad de movimiento |

Todos los secundarios tienen rendimientos decrecientes; Aplomo y Ligereza además tienen tope máximo.

### Árbol de talentos

Un árbol por especialización, 12 en total.

| Aspecto | Decisión |
| --- | --- |
| Estructura | 3 ramas de 6 filas: 18 nodos por árbol |
| Puntos | 20, frente a 33 necesarios para llenarlo |
| Nodos acumulables | 3 por rama, hasta 3 rangos, suman estadísticas pequeñas |
| Nodos de mejora | 2 por rama, cambian una habilidad concreta |
| Nodo de combo | 1 por rama, en la última fila, el más potente |
| Requisito | Cada fila exige puntos gastados en esa rama |
| Reinicio | Gratis fuera de combate |

La decisión de fondo: completar una rama para llegar a su combo, o repartir y ser más versátil.

### Personajes secundarios

Colecciones, monturas, apariencias y renombre son de cuenta; oro y materiales se mueven por el banco de cuenta; los personajes siguientes suben más rápido si ya hay uno al nivel 50.

## Equipo y botín

17 huecos iguales para todas las clases. Cada pieza de jefe tiene estadísticas fijas y conocidas: la aleatoriedad está en si cae o no, y la probabilidad nunca sube.

| Grupo | Huecos |
| --- | --- |
| Armadura visible | Cabeza, hombros, pecho, muñecas, manos, cinturón, piernas, pies, capa |
| Complementos | Collar, anillo, pendiente y dos talismanes |
| Vínculo de alma | Hueco exclusivo de la única legendaria equipable |
| Armas | Mano principal y secundaria, o arma a dos manos |

### Rarezas y estadísticas

| Rareza | Secundarios | Dónde cae |
| --- | --- | --- |
| Común | 0 | Enemigos normales |
| Poco común | 1 | Mundo abierto y misiones |
| Rara | 2 | Mazmorras, misiones importantes, crafteo |
| Épica | 3 más hueco de grabado | Mazmorras difíciles, bandas, PvP |
| Legendaria | 3 más efecto único | Contenido de endgame exigente |

El atributo principal depende del tipo de pieza; los secundarios son fijos por pieza. La reforja permite cambiar un secundario por otro, con coste en Óbolos y materiales, en piezas raras o superiores.

### Reglas de botín

| Tema | Decisión |
| --- | --- |
| Reparto | Botín personal, regalable 30 minutos dentro del grupo y de la misma facción |
| Tablas | Conocidas por jefe y consultables en el juego |
| Mala racha | Sin protección: la probabilidad es siempre la misma |
| Vinculación | Libre, al personaje o a la cuenta |
| Bandas | Botín ilimitado en Normal, bloqueo semanal por jefe en Veterana y Suprema |

### Mejora y personalización

| Sistema | Detalle |
| --- | --- |
| Mejora | +1 a +10 por pieza, nunca falla ni destruye, con techo por temporada |
| Grabados | Un hueco en épicas y legendarias, con runas de Orfebrería |
| Conjuntos | Piezas de banda con bonificación a 2 y 4 que refuerza una rama de talentos |
| Legendarias | Solo una equipada, solo jugando, modifican una habilidad |
| Apariencias | Colección de cuenta: toda pieza conseguida desbloquea su aspecto |
| Durabilidad | Ligera pérdida al morir, se repara con Óbolos |

El equipo sirve para ambas especializaciones, pero sus estadísticas favorecen a una, así que rendir al máximo en las dos exige conseguir piezas distintas. Los conjuntos guardados se cambian con un toque desde el armario y no ocupan espacio en las bolsas.

## Economía y profesiones

Una sola moneda, los Óbolos, con unidad y decimales. El equipo de temporada se compra con Sellos de temporada, que no se comercian.

| Elemento | Cómo se consigue | Para qué sirve |
| --- | --- | --- |
| Óbolos | Misiones, enemigos, recompensas de mazmorras, battlegrounds y Campañas, comercio | Equipo normal, consumibles, mejora, reforja, reparaciones, apariencias básicas |
| Sellos de temporada | Solo nivel 50 y contenido difícil, con tope semanal | Equipo de temporada para el contenido de alto nivel |

Sumideros principales: mejora de equipo, reforja, impuestos del mercado, reparaciones, profesiones, monturas, bolsas y apariencias de oro.

### Gran Mercado

| Regla | Detalle |
| --- | --- |
| Alcance | Común a ambas facciones |
| Comisión | 5 %, más un 10 % extra al comprar a la otra facción |
| Acceso | Comprar y publicar solo en las capitales; las compras y ventas se recogen desde cualquier sitio |
| Depósito | Pequeño coste al publicar, se pierde si no se vende |
| Historial | Precio medio de los últimos días visible |
| Comercio directo | Solo dentro de la misma facción |

### Profesiones

Cada personaje elige una de recolección y una de fabricación. Niveles del 1 al 100.

| Recolección | Qué obtiene |
| --- | --- |
| Minería | Metales, piedras y gemas en bruto |
| Botánica | Plantas, hongos, fibras y maderas |
| Cacería | Pieles, cuero, huesos, carne y seda |

| Fabricación | Qué fabrica |
| --- | --- |
| Forja | Armadura pesada, espadas, hachas, mazas, dagas, armas a dos manos, escudos y bolsas de metal |
| Curtiduría | Armadura media, arcos y bolsas de cuero |
| Sastrería | Armadura ligera, capas, báculos, varitas, tomos, orbes y bolsas de tela |
| Orfebrería | Collares, anillos, pendientes, talismanes y runas de grabado |
| Destilería | Pociones, elixires y frascos |
| Imbuición | Mejoras permanentes sobre piezas; obtiene esencias al disolver piezas |
| Inventor | Mosquetes, artefactos de viaje y utilidades; obtiene piezas al desguazar |

Desguazar es exclusivo del Inventor y disolver de la Imbuición, ambos sobre piezas poco comunes o superiores. El resto de jugadores vende sus piezas a comerciantes o en el Gran Mercado.

El artesano elige los secundarios de lo que fabrica, con un máximo de 2 piezas épicas fabricadas equipadas a la vez. Los consumibles y las runas solo se fabrican, nunca caen de jefes.

### Inventario y anti-bots

Bolsas fabricadas por Forja, Curtiduría y Sastrería con la misma capacidad, bolsa de materiales aparte, armario de conjuntos, banco personal y banco de cuenta. Las cuentas nuevas no pueden comerciar, enviar oro ni usar el mercado hasta el nivel 10, y todo botín, oro y recolección se validan en el servidor.

## PvE

La dificultad es fija y nunca se ajusta a un jugador concreto: cuando hay varias dificultades, es el grupo entero quien elige. No hay NPC acompañantes en ninguna mazmorra.

### Mundo abierto

| Contenido | Detalle |
| --- | --- |
| Historia principal | Cadena de misiones de facción que guía de zona en zona |
| Misiones de zona | Variadas, con poco "matar X enemigos" |
| Misiones de grupo | Opcionales, con mejores recompensas y acceso al buscador; nunca bloquean la historia |
| Jefes de zona | Enemigos poderosos pensados para 2–5 jugadores en coop |
| Brechas | Eventos del Olvido que aparecen en tu mundo individual |
| Bestias raras | Domesticables, solo útiles para el Animalista |
| Secretos | Tesoros, zonas ocultas y logros de exploración |

### Mazmorras

| Nivel | Formato | Duración |
| --- | --- | --- |
| 5–9 | Iniciación muy fácil, 3 jugadores, sin roles | 10–15 min |
| 10–49 | Con roles: 1 tanque, 1 sanador y 1 a 3 de daño | 15–20 min |
| 50 | Igual, con dificultades Normal, Veterana, Suprema y Ascensos | 15–25 min |

Los grupos son de 3 a 5 jugadores y la mazmorra escala para costar lo mismo: con cada jugador de más sube sobre todo la vida de los enemigos y aparecen enemigos adicionales. El tamaño se fija al empezar y el botín es igual con 3 que con 5.

Cada dificultad no solo sube vida y daño: añade habilidades nuevas a jefes y enemigos, lo que obliga a dominar más técnicas.

### Ascensos

Mazmorras escalables sin límite, activadas con un Emblema de Ascenso que cae al completar Supremas.

| Aspecto | Regla |
| --- | --- |
| Niveles | Suben sin tope; cada uno aumenta vida y daño de los enemigos |
| Modificadores | Rotativos por semana, a partir de ciertos niveles |
| Tiempo | Completar a tiempo sube el emblema de nivel; fuera de tiempo lo baja |
| Puntos de Valor | Puntuación PvE de temporada: suma el mejor Ascenso de cada mazmorra |
| Acceso | Solo por grupo manual, no por cola automática |

### Bandas

10 jugadores (2 tanques, 2 sanadores y 6 de daño), 6–8 jefes divididos en alas de 30–45 minutos, con dificultades Normal, Veterana y Suprema y requisito de poder de equipo.

### Buscador y reglas de grupo

| Tema | Regla |
| --- | --- |
| Acceso | Nivel mínimo obligatorio por mazmorra; sin nivel no se entra ni en grupo manual |
| Colas | Aleatoria (con recompensa extra) o mazmorra concreta |
| Rol | Uno por cola, según la clase; la especialización cambia automáticamente al entrar |
| Rol escaso | Más Óbolos para el rol que falta |
| Grupo manual | El líder publica, ve las solicitudes con clase, rol, poder de equipo y Puntos de Valor, y elige |
| Muerte en jefe | No se reaparece hasta que el jefe muere o hay wipe |
| Resurrección | Acción fija de los sanadores, solo fuera de combate, revive a todo el grupo |
| Medidor | Daño, curación, interrupciones y mecánicas visibles para todo el grupo; nunca público |

## PvP

Siempre facción contra facción y siempre instanciado: no hay PvP en el mundo abierto. Todos los modos se ganan por objetivos, y ninguna partida debería pasar de 20 minutos.

| Nivel | Modo | Formatos |
| --- | --- | --- |
| 10–49 | Battlegrounds de subida | 5v5 y 10v10 |
| 50 | Battlegrounds | 10v10 y 15v15 |
| 50 | Campañas | 20v20, con cola propia |
| 50 | Duelos | 2v2 y 3v3 |
| 50 | Clasificatoria | 2v2, 3v3 y 10v10 |

### Mapas

| Mapa | Formato | Cómo se gana |
| --- | --- | --- |
| El Relicario | 5v5 y 10v10 | Captura la bandera, con dos modalidades: bandera propia en cada base, o bandera central cuyo destino cambia durante la partida |
| Tres Piras | 10v10 y 15v15 | Controlar tres piras que dan puntos con el tiempo |
| La Procesión | 10v10 | Escoltar un sarcófago que avanza si tu equipo está cerca |
| Bastión de Ceniza | 15v15 | Asalto por rondas: un equipo ataca y otro defiende, luego se cambian |
| Campos del Olvido | 20v20 | Batalla total: controlar puntos debilita las defensas hasta destruir el estandarte enemigo |

### Reglas de partida

| Regla | Detalle |
| --- | --- |
| Emparejamiento | Por rango de 5 niveles adaptativo durante la subida; por nivel en el 50; por poder de equipo y puntuación en élite y clasificatoria |
| Reaparición | Por oleadas, para volver agrupados |
| Rendición | Mayoría alta a partir de la mitad de la partida; nunca en duelos |
| Abandono | Penalización creciente de 5 minutos a 4 horas, con 2 minutos de margen para reconectar |
| Inactividad | Aviso a los 90 segundos y expulsión con la misma penalización |
| Desgaste | Solo en duelos y clasificatoria 2v2 y 3v3: desde el minuto 3 baja toda la curación |
| Facción minoritaria | Más Óbolos y experiencia, sin tocar Sellos ni rango |
| Comunicación | Chat con tu equipo y pings; nada con el rival |

### Clasificatoria

Puntuación continua y visible, independiente para cada formato.

| División | Puntuación |
| --- | --- |
| Hierro | 0–499 |
| Bronce | 500–999 |
| Plata | 1.000–1.499 |
| Oro | 1.500–1.999 |
| Élite | 2.000–2.499 |
| Leyenda | 2.500 o más y top 500 de la región |

Cada división tiene tres secciones (III, II, I) salvo Leyenda, que muestra la posición exacta. Los puntos por partida varían según la puntuación del rival, y al alcanzar una división nueva no se puede bajar durante 5 partidas. Las recompensas de temporada son de prestigio: títulos, monturas, apariencias y estandartes.

### Legibilidad

Aliados en azul y enemigos en rojo con modo daltónico, icono sobre los sanadores, marca visible del portador de bandera, efectos de aliados reducidos por defecto en 10v10 o más, y marcador siempre a la vista. Bajar la calidad gráfica nunca oculta información ni da ventaja.

## Sistemas sociales

La vida social ocurre en las capitales y en el contenido de grupo. No hay chat de voz: los pings y el chat de grupo lo cubren.

| Sistema | Reglas |
| --- | --- |
| Amigos | Por nombre dentro de la facción o por código de jugador entre facciones; susurro permitido solo entre amigos confirmados |
| Recientes | Lista automática de jugadores con los que has jugado, para volver a invitarlos |
| Grupos | De 2 a 5 jugadores y bandas de 10, mixtos, con chat de grupo, pings y marcadores del líder |
| Grupo manual | El líder publica en el tablón, ve quién se apunta y elige a quién invita |
| Bloqueo | Impide invitaciones, mensajes y coincidir en emparejamiento |

### Pings

Sistema estilo League of Legends adaptado al táctil: botón junto al minimapa con toque rápido, rueda al mantener y arrastrar, y ping directo sobre el minimapa. Pings inteligentes según lo que marques (enemigo, aliado, objeto, habilidad propia) y rueda con peligro, voy en camino, necesito ayuda, reagrupar, enemigo desaparecido y esperad. Marcadores persistentes del líder en mazmorras y bandas. Límite anti-spam y opción de silenciar los pings de un jugador. En PvP solo los ve tu equipo.

### Gremios

| Aspecto | Decisión |
| --- | --- |
| Facción | Una sola |
| Tamaño | 100 cuentas, no personajes |
| Nivel | Sube por la actividad en grupo de sus miembros; lo jugado en solitario no cuenta |
| Recompensas | Banco por pestañas, calendario, emblema, estandarte, tabardo, montura, título y descuento en reparaciones |
| Sin poder | Ninguna mejora de gremio afecta a las estadísticas |
| Clasificación | Tabla por región: progreso de banda, Puntos de Valor y victorias en PvP |

### Capitales

Valdor y Osvar se reparten en copias de unos 50 jugadores, con salto automático a la copia donde estén tus amigos o tu gremio. El chat de gremio, la lista de amigos y el Gran Mercado son globales.

| Zona | Qué hay |
| --- | --- |
| Plaza central | Punto de reunión, tablón de anuncios y marcadores de guerra y del Olvido |
| Barrio de profesiones | Entrenadores y estaciones de todas las profesiones |
| Zona de equipamiento y pruebas | Apariencias, reforja, mejora y muñecos de entrenamiento |
| Gran Mercado y banco | Compra, venta y almacenamiento |
| Zona de duelos | Duelos amistosos dentro de la misma facción |

Los muñecos son de cuatro tipos (inmóvil, que devuelve daño, con armadura alta y en movimiento) y muestran un medidor de tu propio personaje con desglose por habilidad.

### Perfil y moderación

El perfil muestra raza, clase, especialización, poder de equipo, Puntos de Valor, mejor Ascenso por mazmorra, progreso de banda, divisiones de PvP por formato, colecciones, gremio y títulos, con opción de ocultarlo a desconocidos.

Moderación desde el lanzamiento: filtro de palabras activo por defecto, silenciar a un jugador incluidos sus pings, reportar desde el chat, el perfil y el final de partida, sanciones escaladas, filtro de nombres y restricciones para cuentas de menores.

## Arte y audio

Estilo estilizado semirrealista con un punto oscuro: proporciones creíbles pero algo heroicas, siluetas fuertes y texturas limpias. Es la opción que mejor rinde en móvil, la más legible en pantalla pequeña y la que mejor envejece.

| Elemento | La Llama Blanca | El Pacto Oscuro |
| --- | --- | --- |
| Colores | Blanco hueso, dorado y rojo brasa | Negro, gris ceniza y verde o violeta espectral |
| Materiales | Metal pulido, tela limpia, reliquias, velas | Hueso, cuero curtido, metal ennegrecido, telas raídas |
| Luz | Cálida y direccional | Fría, con espíritus flotando |
| Arquitectura | Piedra clara, arcos altos, torres con fuegos encendidos | Necrópolis y criptas, con hogueras de fuego negro que absorben la luz |
| Formas | Verticales y simétricas | Irregulares, orgánicas, con hueso integrado |

El fuego negro necesita contorno luminoso para leerse sobre fondos oscuros. El Olvido tiene un color propio que nadie más usa y que apaga el color de todo lo que toca.

### Efectos de combate

| Regla | Detalle |
| --- | --- |
| Zonas de peligro | Siempre visibles, con el mismo lenguaje visual en todo el juego |
| Avisos | Al menos 1,5 segundos antes, con sonido |
| Efectos de aliados | Reducibles, y reducidos por defecto en 10v10 o más |
| Calidad gráfica | Bajarla nunca oculta información ni da ventaja |

### Audio

Orquestal con instrumentos de época: coros en la Llama Blanca, percusión y voces graves en el Pacto Oscuro. Música dinámica entre exploración, combate y jefe, con tema propio por región que se apaga al ascender. Voces solo en personajes clave, frases cortas por raza y sexo, vibración corta en impactos importantes.

Regla básica: el juego debe funcionar sin sonido, así que todo aviso sonoro lleva su equivalente visual.

## Controles e interfaz

Orientación horizontal siempre, también en menús, y juego a dos manos: pulgar izquierdo al movimiento, derecho a las habilidades.

| Zona | Elementos |
| --- | --- |
| Izquierda | Joystick flotante que aparece donde se toca; cámara por arrastre y pellizco para acercar |
| Derecha | Ataque básico grande, 6 habilidades en arco, definitiva y esquiva separadas, botón de salto |
| Extra | Alimentar bestia con anillo de vida (Animalista) y resurrección fuera de combate (sanadores) |
| Arriba izquierda | Tu retrato, vida, recurso y efectos; debajo, los retratos del grupo, tocables para curar |
| Arriba centro | Objetivo actual, su vida y lo que está lanzando |
| Arriba derecha | Minimapa y botón de pings |
| Plegables | Chat abajo a la izquierda y medidor de daño y curación |

Todos los botones se pueden mover, escalar y reordenar, con perfiles distintos por especialización.

### Menú y equipo

Un solo menú con pestañas: personaje, habilidades y talentos, bolsas, mapa, misiones, social, actividades, colecciones, profesiones y tienda.

En la gestión de equipo: comparación directa con la pieza equipada, botón de equipar lo recomendado para la especialización activa, filtros por hueco y rareza, marcar como basura para vender en bloque y cambio de conjunto con un toque.

### Ajustes y rendimiento

| Categoría | Opciones |
| --- | --- |
| Gráficos | Presets y ajustes sueltos, límite de FPS, ahorro de batería |
| Combate | Prioridad de selección, apuntado, tamaño y posición de botones |
| Efectos | Reducir los de aliados y ocultar cosméticos de otros jugadores |
| Accesibilidad | Modo daltónico, tamaño de texto, modo zurdo, subtítulos, vibración |
| Social | Filtro de chat, invitaciones, susurros, privacidad del perfil |
| Datos | Aviso al jugar sin wifi y modo de bajo consumo |

Objetivo de rendimiento: 60 FPS en gama media y 30 estables en gama baja, con reconexión sin penalización en menos de 2 minutos y descarga por partes.

### Onboarding

Combate con dos botones en el primer minuto, introducción de raza hasta el 10, capital en el 3, primera mazmorra en el 5 y elección de especialización en el 10 con explicación clara de los roles. Tutoriales cortos, contextuales, saltables y consultables después.

## Monetización y live ops

Juego gratuito con cosméticos, pase de temporada y comodidades. Sin moneda premium: cada producto se paga con su precio directo en euros a través de la tienda de Android.

### La línea roja

Nunca se vende equipo, mejoras, Sellos de temporada, Óbolos, ventajas en combate, experiencia que salte contenido, cajas de botín al azar ni bestias con ventaja. Es el principal argumento frente a la competencia en móvil.

### Qué se vende

| Categoría | Ejemplos |
| --- | --- |
| Apariencias | Conjuntos, apariencias de arma, efectos de espalda, tintes |
| Monturas | Solo estéticas |
| Pase de temporada | Vía gratuita y vía de pago, esta solo con cosméticos y comodidades |
| Comodidades | Espacio de banco, huecos de personaje, cambio de nombre o aspecto |
| Cambio de raza | Dentro de la facción y compatible con la clase |
| Cambio de facción | Solo hacia la facción minoritaria |
| Paquetes temáticos | Conjunto, montura y título a juego |

### Pase de temporada

Progresa jugando cualquier contenido y se completa con una hora al día. Nada de lo que da sube el poder de equipo, y no hay penalización por no comprarlo: nunca debe empujar a jugar a diario bajo amenaza de perder algo.

### Tienda

Rotación por temporadas y eventos, nunca accesible durante el combate y sin anuncios emergentes. La escasez se usa por edición limitada (un conjunto de una temporada que después no vuelve), no con contadores cortos ni ofertas que interrumpan la sesión. Control parental con límite de gasto y compras restringidas en cuentas de menores.

### Live ops

| Ritmo | Contenido |
| --- | --- |
| Temporadas | Cada 3 meses: historia del Olvido, techo de mejora, temporada de PvP y de Puntos de Valor |
| Parches | Cada 4–6 semanas: equilibrio, arreglos y contenido menor |
| Eventos | Festividades de facción, semanas de Campañas, eventos de Brechas |
| Gran actualización | Anual: nivel máximo, zona nueva, mazmorras y banda |

## Producción, MVP y riesgos

Lo diseñado es un proyecto grande: un estudio profesional lo abordaría con 25–60 personas durante 3 o 4 años. La ruta elegida es completar el diseño primero y decidir después, construyendo por cuenta propia.

| Elemento | Volumen diseñado |
| --- | --- |
| Modelos base | 12 (6 razas × 2 sexos) |
| Especializaciones | 12, con unas 120 habilidades y 24 definitivas |
| Árboles de talentos | 12, con 216 nodos (108 tocan código de habilidades) |
| Zonas | 6 introducciones, 4 de facción, 6 compartidas y la Quiebra |
| Mazmorras | 13 |
| Bandas | 1, con varias alas |
| Mapas de PvP | 5 |
| Profesiones | 3 de recolección y 7 de fabricación |

### Prototipo vertical

| Entra | Detalle |
| --- | --- |
| Razas | Valdrenos y Vorkrath |
| Clases | Conquistador y Templario/Juramentado (tanque y daño) |
| Niveles | 1 a 10 |
| Mundo | Una zona pequeña |
| PvE | Una mazmorra de 3 jugadores, sin rol de sanador |
| PvP | Un battleground 5v5 |
| Sistemas | Combate híbrido, equipo básico, pings y buscador simple |
| Fuera | Talentos, profesiones, mercado, gremios, bandas, Ascensos y pase |

Si más adelante se quiere probar el trío completo de roles, la clase a añadir es el Alquimista, sanador compartido por ambas facciones.

### Orden de construcción

1. Prueba de combate: un personaje, unos enemigos y 6 habilidades en un móvil real. Es el paso más barato y el que decide si el juego funciona.
2. Prueba de red: dos jugadores en la misma mazmorra con el servidor validando.
3. Bucle completo: matar, subir, conseguir equipo, mejorar.
4. Prototipo vertical.
5. Pruebas con jugadores y medición de retención.

### Riesgos

| Riesgo | Mitigación |
| --- | --- |
| Población insuficiente | Formatos pequeños primero, rangos adaptativos, lanzamiento por regiones |
| Alcance excesivo | Prototipo antes que contenido; es la causa número uno de proyectos abandonados |
| Coste de arte | Reutilización, esqueletos compartidos y estilo estilizado |
| Rendimiento en 20v20 | Probarlo pronto con muchos personajes en pantalla |
| Trampas y bots | Validación por servidor desde el diseño |
| Comparación con WoW | Nombres, sistemas y lore propios |
| Retención en móvil | Sesiones cortas y primeros minutos muy cuidados |

### Legal y métricas

Comprobar Umbralis en EUIPO, OEPM y Google Play, y reservar dominio y perfiles. Cumplir RGPD, clasificación PEGI previsible de 12 o 16 y normas de compra de Google Play. Referencias razonables en móvil: retención D1 del 35–40 %, D7 del 15–20 %, D30 del 6–10 %, sesión media de 20–30 minutos y conversión a pago del 2–5 %.

## Pendientes

El diseño está cerrado en todos sus bloques. Lo que queda se decide al bajar al detalle de cada pieza.

| Pendiente | Cuándo se resuelve |
| --- | --- |
| Nombres de zonas, la Quiebra y personajes clave | Al diseñar cada zona |
| Mecánicas de cada jefe | Al diseñar cada mazmorra y la banda |
| Diseño de la primera mazmorra y el primer battleground | Al preparar el prototipo |
| Números concretos: daño, vida, curvas, precios y umbrales de división | En las pruebas de equilibrio |
| Comprobación y registro de la marca Umbralis | Antes de publicar |
| Motor, arte y ayuda externa | Antes de empezar la prueba de combate |

### Próximo paso

La prueba de combate en un móvil real: un personaje, unos enemigos y seis habilidades. Es lo más barato de construir y lo que decide si el resto del proyecto merece la pena.
