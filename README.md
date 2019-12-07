
# Tongued
![alt text](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Logos/img_tongued_logo.png)

# Índice
- [Concepto general](#concepto-general)
- [Mecánicas](#mecánicas)
  * [Movimiento](#movimiento)
  * [Controles](#controles)
- [Cámara](#cámara)
- [Interacción entre Jugadores](#interacción-entre-jugadores)
- [Modos de Juego](#modos-de-juego)
  * [Bendición y Maldición](#bendición-y-maldición)
  * [Otros](#otros)
- [Mapas](#mapas)
- [Modelo de Negocio](#modelo-de-negocio)
  * [¿Cómo Generamos Dinero? ](#cómo-generamos-dinero)
  * [La Tienda](#la-tienda)
  * [Modos de juego y su función en el Modelo](#modos-de-juego-y-su-función-en-el-modelo)
  * [Divisas del juego](#divisas-del-juego)
  * [El Usuario Premium en resumen](#el-usuario-premium-en-resumen)
- [Diagrama de flujo](#diagrama-de-flujo)
- [Estilo Menús](#estilo-menús)

# Concepto General
Tongued es un juego multijugador en el que los jugadores, controlando a unos simpáticos pero competitivos camaleones, se enfrentan en partidas todos contratodos para ganar la partida. Para ello, usarán la lengua del camaleón para engancharse, balancearse y lanzarse a por el mapa, aprovechando la inercia que sus movimientos. 

![captura juego](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Capturas%20de%20pantalla/image(6).png)
# Mecánicas
## Movimiento
El jugador puede lanzar su lengua como si fuera un gancho. Para hacerlo, selecciona aproximadamente el punto al que quiere engancharse (esto se explica unas líneas mas abajo). La punta de la lengua viaja rápidamente hacia esta dirección. Una vez choca con una pieza del escenario, el jugador cuelga de su lengua, como si fuera una cuerda. La longitud de esta cuerda será la que tuviera en el momento en el que se enganchó.

La lengua tiene colisiones en toda su extensión, puediéndose esta enrollar alrededor o crear nuevos ejes de giro en los elementos del mapa. Esto proporciona un movimiento más realista pues aporta a los jugadores la capacidad de ganar más velocidad, hacer quiebros o movimientos no previstos.
Habrá una lógica permisiva para decidir dónde va la lengua, tratando el punto exacto que ha solicitado el jugador como una "sugerencia" y buscando algo a lo que engancharse en esa dirección general.
## Controles
Respecto a los controles, hay dos tipos, uno de ellos muy enfocado en el juego en dispositivos táctiles, aunque puede ser usado también con ratón.
En todo momento, el camaleón del jugador se comporta como un objeto de físicas, manteniendo su inercia, rebotando contra el escenario, etc.
### Modo Manual
- Para lanzar la lengua haciendo click izquierdo en la posición deseada.
- Puede retractar la lengua  manteniendo click izquierdo, disminuyendo la longitud de la "cuerda".
- Puede soltar el gancho haciendo click derecho.

### Modo Arcade/Táctil
- Para lanzar la lengua tocando y manteniendo el dedo (o el botón izquierdo del ratón) en la posición deseada.
- Puede retractar la lengua arrastrando el dedo o el cursor (sin haberlo levantado tras lanzar) alejándolo de la posición incial, disminuyendo la longitud de la "cuerda".
- Puede soltar el gancho levantando el dedo de la pantalla o soltando el botón izquierdo del ratón.

# Cámara
La cámara está relativamente alejada, pudiendo ver los alrededores de la posición del jugador, pero no enseña todo el escenario al mismo tiempo.
La cámara sigue al jugador, intentando colocarse un poco por delante de él en base a su velocidad.
Se agita cuando te enganchan o al jugador le sucede algo que le repercute de forma negativa.

# Interacción entre jugadores
La permisividad a la hora de decidir dónde va la lengua dejará de actuar cuando un oponente esté potencialmente en el camino. En ese caso, la lengua irá exactamente a donde el jugador ha decidido, obligándole a apuntar para enganchar a un oponente.

Cuando un jugador consigue enganchar a otro, la víctima pierde una gran proporción de su velocidad, mutiplicándola por un factor reductor, y resepecto al jugador que ha conseguido enganchar al otro su lengua queda fijada a este pudiéndola retractar para alcanzarle.

Los jugadores no colisionan entre sí de una manera física, pero si que pueden interactuar como se detalla en los modos de juego. 

Solo las puntas de las lenguas pueden colisionar entre sí, cancelando la trayectoria que llevaban ambas, puediendo así bloquear a rivales que pretenden enganchar al jugador u otros elementos.

# Modos de juego
## Bendición y Maldición
![Bendicion y Maldicion](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/GDD/img_promo_ByM.png)
- El modo central del juego.
- La partida dura 10 rondas.
- Varios jugadores en el mismo escenario, todos contra todos. Al empezar la partida, hay una pequeña cuenta atrás para dar la oportunidad a los jugadores de comenzar a moverse y ganar velocidad. AL terminar la cuenta atrás, una gema en un lugar aleatorio del mapa de entre 7 posibles. Cuando un jugador pasa sobre la gema, la coge. En ese momento, la barra de tiempo comienza a vaciarse. Al perder la gema la barra se reinicia. El jugador que complete la barra gana o pierde un punto según el tipo de ronda. 
- Para controlar que una ronda, y por tanto la partida, no sea eterna, el tiempo a mantener la gema pasado cierto punto de la ronda, comienza a ser menor. La función principal de esto es evitar partidas excesivamente largas y que la acción del juego no se desarrolle justo al final de una ronda, haciendo que los jugadores no tengan un comportamiento que arruine el juego.
- Tras finalizar una ronda empieza una nueva. Existen 2 tipos de ronda:
  Ronda de Bendición: el jugador que termine la ronda con el objeto, gana un punto.
  Ronda de Maldición: el jugador que termine la ronda con el objeto, pierde un punto.
- Siempre se inicia con la ronda de bendición, para hacer que siempre un jugador tenga el objeto.
- Estos dos tipos de ronda se van alternando de una manera aleatoria con cierto matiz. Tras acabar una ronda, la siguiente se elige de forma aleatoria, aunque para evitar monotonía, el repetir el mismo tipo de ronda cada vez tiene menos probabilidades respecto al otro (la mitad).
- Para quitar o pasar el objeto a alguien (dependiendo del tipo de ronda), los dos jugadores deben entrar en contacto. Cuando entren en colisión, se transferirá el objeto del uno al otro.

![Bendicion](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Capturas%20de%20pantalla/image(7).png)

![Maldicion](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Capturas%20de%20pantalla/image(8).png)

## Otros
### Carrera de Comida
- Es una propuesta de futuro, no está en el juego base.
- Varios jugadores en el mismo escenario, todos contra todos. 
- Aparece un objeto (moscas, o algo por el estilo. Comida.) en un punto predefinido del escenario.
- Cuando un camaleón pasa por este, se lo "come". Al comer, el camaleón se hace más grande; haciéndole más fácil de enganchar.
- Cada vez que el objeto se come, aparece otro objeto en un punto aleatorio de varios predefinidos.
- Gana el primer jugador en llegar a X objetos comidos.
### Modo Fin de Semana
- Modo con entrada a cambio de tickets.
- Se ganan gemas.
- Solo fines de semana.
### Aventura de Temporada
- Modos tanto multijugador como de un jugador especiales con la temática de la temporada.

# Mapas
## Selva
El mapa principal.
Cuenta con elementos a los que engancharse como con troncos de diferentes tamaños o rocas de forma cuadrada con esquinas por toda su extensión. Los límites del mapa están marcados con dos troncos a ambos lados y maleza arriba, todos ellos enganchables. Toda la parte inferior está cubierta por agua, en la que podemos sumergirnos, afectando esto a nuestra inercia.

## Gravedad puntual
Propuesta para el futuro. Mapa circular en el que la gravedad no es direccional, si no que es hacia un punto central.
Temáticamente, podría ser un planeta muy pequeño, o los camaleones pueden haber bajado al núcleo de la tierra. Cuantos más assets se puedan reutilizar mejor. Podemos utilizar iluminación diferente/un filtro para diferenciar más los mapas visualemente.

# Modelo de Negocio
## ¿Cómo generamos dinero?
El juego es free to play. El dinero se genera con la venta de cosméticos y un sistema de usuario premium, a modo de pase de temporada.

## La tienda
En la tienda solo se podrán comprar cosméticos, oro, gemas, pases y el pase de temporada. No existen ventajas sobre el juego de ningún tipo.

## Modos de juego y su función en el Modelo
- Partida Normal: modos clásicos dónde cumplir desafíos. Ganas oro y pases (usuario premium). Se entra gratis.
- Evento de Fin de Semana: modo fijo multiplayer o con rotación de unos pocos. Se entra con pases.
- Aventura de Temporada: eventos singleplayer con mas "historia". Tematización por temporada.

## Divisas del juego
![divisas](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/GDD/img_promo_divisas.png)
- Oro: compra de skins básicas, compra de pases de evento. Se ganan en desafios base, recompensas de partida normal o con gemas.
- Gemas: moneda comprable con dinero real. Convertible en oro. También se ganan en eventos y aventuras.
- Tickets: utilizables para jugar eventos. Comprables con oro y conseguibles completando desafios Premium.

## El Usuario Premium en resumen
- Desafios Premium extra: dan pases de evento de fin de semana (por tanto mayor accesibilidad a este modo).
- Aventuras exclusivas.
- Cosméticos exclusivos (solo a cambio de gemas, por tanto sólo conseguibles con facilidad y rentabilidad por usuarios premium).
# Diagrama de Flujo
![diagrama](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/GDD/diagrama.png)
# Estilo Menús
## Menú Principal
![mainmenu](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Capturas%20de%20pantalla/menu.png)
## Sala previa a la partida
![sala](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Capturas%20de%20pantalla/image(10).png)
## Pantalla de Puntuaciones
![scores](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Capturas%20de%20pantalla/image(9).png)
## Ajustes
![ajustes](https://github.com/FresisuisHunters/Camaleones/blob/master/Concept%20Art/Capturas%20de%20pantalla/opciones.png)
