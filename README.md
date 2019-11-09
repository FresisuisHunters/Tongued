
# Camaleones
![alt text](https://i.redd.it/rdzla6wc32z01.jpg)

Las parántesis con iterrogación denotan detalles necesitan ser prototipados antes de tomar la decisión final.

# Movimiento
El jugador puede lanzar su lengua como si fuera un gancho. Para hacerlo, toca el punto (o hace click) al que quiere engancharse. La punta de la lengua viaja rápidamente hacia esta dirección. Una vez choca con una pieza del escenario, el jugador cuelga de su lengua, como si fuera una cuerda. La longitud de esta cuerda será la que tuviera en el momento en el que se enganchó.

Habrá una lógica permisiva para decidir dónde va la lengua, tratando el punto exacto que ha solicitado el jugador como una "sugerencia" y buscando algo a lo que engancharse en esa dirección general. 

Puede retractar la lengua manteniendo el dedo sobre la pantalla (o manteniendo click izquierdo), disminuyendo la longitud de la "cuerda".
Puede soltar el gancho tocando la pantalla dos veces rápidamente (¿tal vez mejor un pequeño botón dedicado para ello?) (o haciendo click derecho).

En todo momento, el camaleón del jugador se comporta como un objeto de físicas, manteniendo su inercia, rebotando contra el escenario, etc...

# Cámara
La cámara está relativamente alejada, pudiendo ver bastante lejos, pero no enseña todo el escenario al mismo tiempo.
La cámara sigue al jugador, intentando colocarse un poco por delante de él en base a su velocidad.
Se agita cuando te enganchan. (¿Se agita también cuando enganchas a alguien?)

# Interacción entre jugadores
La permisividad a la hora de dedcidir dónde va la lengua dejará de actuar cuando un oponente esté potencialmente en el camino. En ese caso, la lengua irá exactamente a donde el jugador ha decidido, obligándole a apuntar para enganchar a un oponente.

Cuando un jugador consigue enganchar a otro, el jugador enganchado pierde una gran proporción de su velocidad, mutiplicandola por un factor alrededor de 0.2 (¿tal vez en vez de eso, su velocidad paa ser una muy baja en dirección al jugador que lo ha enganchado?), y el jugador que ha conseguido enganchar al otro recibe un impulso en dirección al oponente (¿tal vez en vez de una fuerza, se cambia directamente su velocidad por una en dirección al oponente?).

Los jugadores no colisionan entre sí. 

(¿Las puntas de las lenguas pueden chocar y enghancharse una a la otra?)

# Modos de juego
## Rey de la pista/Patata caliente
- Lo primero de todo, hace falta un nombre mejor.
- El modo central del juego.
- Varios jugadores en el mismo escenario, todos contra todos. Al empezar la partida, hay un objeto en el centro del escenario. Cuando un jugador pasa por él, lo coge. En ese momento, empieza una cuenta atrás de unos 20 segundos. El jugador que tenga el objeto al final de la cuenta atrás gana un punto.
- Al final de esta cuenta atrás, empieza una nueva. Al final de esta cuenta atrás, el jugador que tenga el objeto PERDERÁ un punto. El jugador que tenía el objeto no lo pierde. El estado de los jugadores no se reinicia.
- Estos dos modos se van alternando hasta que termina la partida tras X rondas.
- Para quitar el objeto/dárselo a alguien (dependiendo del modo actual), los dos jugadores deben entrar en contacto. Cuando entren en colisión, se transferirá el objeto del uno al otro.

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
Estaría bien tener un mapa circular en el que la gravedad no es direccional, si no que es hacia un punto central.
Temáticamente, podría ser un planeta muy pequeño, o los camaleones pueden haber bajado al núcleo de la tierra. Cuantos más assets se puedan reutilizar mejor. Podemos utilizar iluminación diferente/un filtro para diferenciar más los mapas visualemente.

