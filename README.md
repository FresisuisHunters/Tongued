
# Camaleones
![alt text](https://i.redd.it/rdzla6wc32z01.jpg)

Las parántesis con iterrogación denotan detalles necesitan ser prototipados antes de tomar la decisión final.

# Movimiento
El jugador puede lanzar su lengua como si fuera un gancho. Para hacerlo, selecciona el punto al que quiere engancharse (aproximadamente, esto se explica unas líneas mas abajo). La punta de la lengua viaja rápidamente hacia esta dirección. Una vez choca con una pieza del escenario, el jugador cuelga de su lengua, como si fuera una cuerda. La longitud de esta cuerda será la que tuviera en el momento en el que se enganchó.

La lengua tiene colisiones en toda su extensión, puediéndose esta enrollar alrededor o crear nuevos ejes de giro en los elementos del mapa. Esto proporciona un movimiento mas realista pues aporta a los jugadores la capacidad de ganar más velocidad, hacer quiebros o movimientos no previstos, afectando al principio casi como un obstáculo mas pero si es dominado puede hacer aún mas satisfactorio el moverse bien.

Habrá una lógica permisiva para decidir dónde va la lengua, tratando el punto exacto que ha solicitado el jugador como una "sugerencia" y buscando algo a lo que engancharse en esa dirección general.

Respecto a los controles, hay dos tipos, uno de ellos muy enfocado en el juego en dispositivos táctiles, aunque puede ser usado también con ratón.

Modo Manual:
Para lanzar la lengua haciendo click izquierdo en la posición deseada.
Puede retractar la lengua  manteniendo click izquierdo, disminuyendo la longitud de la "cuerda".
Puede soltar el gancho haciendo click derecho.

Modo Arcade/Táctil:
Para lanzar la lengua tocando y manteniendo el dedo (o el botón izquierdo del ratón) en la posición deseada.
Puede retractar la lengua arrastrando el dedo o el cursor (sin haberlo levantado tras lanzar) alejándolo de la posición incial, disminuyendo la longitud de la "cuerda".
Puede soltar el gancho levantando el dedo de la pantalla o soltando el botón izquierdo del ratón.

En todo momento, el camaleón del jugador se comporta como un objeto de físicas, manteniendo su inercia, rebotando contra el escenario, etc...

# Cámara
La cámara está relativamente alejada, pudiendo ver bastante lejos, pero no enseña todo el escenario al mismo tiempo.
La cámara sigue al jugador, intentando colocarse un poco por delante de él en base a su velocidad.
Se agita cuando te enganchan o al jugador le sucede algo que le repercute de forma negativa.

# Interacción entre jugadores
La permisividad a la hora de dedcidir dónde va la lengua dejará de actuar cuando un oponente esté potencialmente en el camino. En ese caso, la lengua irá exactamente a donde el jugador ha decidido, obligándole a apuntar para enganchar a un oponente.

Cuando un jugador consigue enganchar a otro, la víctima pierde una gran proporción de su velocidad, mutiplicándola por un factor reductor, y resepecto al jugador que ha conseguido enganchar al otro su lengua queda fijada a este pudiéndola retractar para alcanzarle.

Los jugadores no colisionan entre sí de una manera física, pero si que pueden interactuar como se detalla en los modos de juego. 

Solo las puntas de las lenguas pueden colisionar entre sí, cancelando la trayectoria que llevaban ambas, puediendo así bloquear a rivales que pretenden enganchar al jugador u otros elementos.

# Modos de juego
## Bendición y Maldición
- El modo central del juego.
- Existe una cuenta atrás que controla la duración de la partida.
- Varios jugadores en el mismo escenario, todos contra todos. Al empezar la partida, hay un objeto en el centro del escenario. Cuando un jugador pasa por él, lo coge. En ese momento, empieza una cuenta atrás de unos segundos. El jugador que tenga el objeto al final de la cuenta atrás gana o pierde un punto. 
- En los últimos segundos de la ronda se entra en la prórroga, un tiempo en el que si el objeto es robado, el tiempo de la prórroga se reinicia. El tiempo de la prórroga cada vez es menor (aunque tendrá un mínimo) para controlar que un partida no sea eterna. La función principal de la prórroga es evitar que la acción del juego se desarrolle justo al final de una ronda, haciendo que los jugadores no tengan un comportamiento que arruine el juego.
- Tras finalizar una ronda empieza una nueva. Existen 2 tipos de ronda:
  Ronda de Bendición: el jugador que termine la ronda con el objeto, gana un punto.
  Ronda de Maldición: el jugador que termine la ronda con el objeto, pierde un punto.
- Siempre se inicia con la ronda de bendición, para hacer que siempre un jugador tenga el objeto.
- El estado de los jugadores no se reinicia.
- Estos dos modos se van alternando (?) hasta que termina la partida tras X rondas.
- Para quitar el objeto/dárselo a alguien (dependiendo del tipo de ronda actual), los dos jugadores deben entrar en contacto. Cuando entren en colisión, se transferirá el objeto del uno al otro.

## Carrera de comida
- El nombre se puede mejorar. El modo también. Es una propuesta.
- Varios jugadores en el mismo escenario, todos contra todos. 
- Aparece un objeto (moscas, o algo por el estilo. Comida.) en un punto predefinido del escenario.
- Cuando un camaleón pasa por este, se lo "come". Al comer, el camaleón se hace más grande; haciéndole más fácil de enganchar. (¿Y si resulta que ser grande es una ventaja porque es más fácil pasar por la comida?)
- Cada vez que el objeto se come, aparece otro objeto en un punto aleatorio de varios predefinidos.
- Gana el primer jugador en llegar a X objetos comidos.

# Mapas
## Selva
El principal. La gravedad va, sorprendentemente, hacia abajo.
Varias posibilidades para los que hay abajo: 
- Suelo contra el que los camaleones rebotan.
- Suelo que no rebota
- Agua que mata tu velocidad horizontal, pero que te lanza un poco hacia arriba.

## Gravedad puntual
Mapa circular en el que la gravedad no es direccional, si no que es hacia un punto central.
Temáticamente, podría ser un planeta muy pequeño, o los camaleones pueden haber bajado al núcleo de la tierra. Cuantos más assets se puedan reutilizar mejor. Podemos utilizar iluminación diferente/un filtro para diferenciar más los mapas visualemente.

