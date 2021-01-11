# Ficha de juego


#### Título  
Polo Hunt 
#### Género 
Es un juego multijugador en 3D con ....
#### Audiencia
Es un juego pensado para todos los públicos
#### Plataformas
En ordenador y disporsitivos móviles
#### Modos de Juego
El juego consta de un
#### Temática
Los paisajes del juego están inspirados en el ártico. 
Hemos decidido obviar las disonancias que hay entre los distintos animales situados en cada polo y hacer un juego en el que todos sus elementos sean procedentes de lugares helados independientemente de la coherencia de juntar a estos.
[Revisar esto ]
## Descripción del juego
Polo Hunt es un juego multijugador diseñado para todos los públicos, que consta de unos diversos modos de juego pensados para divertir a los jugadores haciendo gala de esta singular temática polar.

Es un juego pensado para hacer una ligera crítica al cambio climático. En el hay diversos modos de juegos para participar, aunque la esencia de este y de varios de sus modos es el clásico escondite/pilla pilla.(Revisar)

Los personajes principales son pingüinos y osos polares. Los osos serán los encargados de dar caza a todos los pingüinos, estos tendrán que salvarse escondiendose por el mapa y usando las distintas habilidades que poseen. Para que el juego resulte atractivo su diseño se centra en el diseño de escenarios, el movimiento de los personajes y las habilidades de estos para huir y atrapar.

El desarrollo de una partida normal sigue la dinámica del clásico juego pilla pilla. El oso hace una cuenta atrás con los ojos cerrados y mientras los pingüinos recorren el mapa buscando un buen escondite. Una vez el oso termine de contar este va a dar caza a las aves usando su habilidad especial  y su ventaja a la hora de correr más rápido. En el modo principal una vez el oso atrapa a un jugador, este desaperece (esto cambia en función al modo), el flujo de la partida está limitado por dos condiciones, el tiempo establecido o la victoria del oso sobre los otros. 

El escenario está ideado para poder explotar las habilidades de ambos personajes y esconderse o despistar al oso en muchos sitios. 

Aunque el pilla pilla sea su modo principal, hay un modo de carreras y un modo de sumo también.


...


## Personajes
Como personajes protagonistas del juego hemos optado por los pingüinos, ya que estos eran la esencia del tipo de movimiento y físicas que queríamos transmitir a los jugadores. Gracia y toperza, estos son los dos grandes adjetivos que son capaces de sintetizar la esencia de nuestro juego.
Gracias al movimiento de estos animales y observando las graciosas caidas que tienen en el hielo, hemos determinado que son el animal ideal para representar el tipo de jugabilidad que queríamos ofrecer. 

Aquí se muestra la hoja del pingüino base que hay en el juego.

![Pingas](https://user-images.githubusercontent.com/55508821/101229277-b76ce880-369f-11eb-8cf7-dd9dc3491c90.jpeg)


El oso polar era una de las grandes opciones que barajábamos como posibles enemigos. Este ha sido el animal escogido para dar caza debido a que la fisonomía de este era el más plausible a la hora de desarrollar, teniendo como premisa que el rol de cazador lo debía ejecutar también un animal relacionado con el hielo.


![ositos](https://user-images.githubusercontent.com/55508821/101229282-c05dba00-369f-11eb-99f0-5ebefa601b05.jpeg)


## Modos de juego

La esencia de nuestro juego y su modo principal es cacería, el clásico pilla pilla. Aquí los osos perseguirán a los pingüinos hasta que se acabe el tiempo o estos hayan dado caza a todos.
En función a este modo pricipal, hemos desarrollado varios de este estilo:
- Supervivencia: En esto modo solo habrá uno o dos pingüinos, estos dispondrán de un aumento de velocidad para compensar su inferioridad numérica.
- "Zombie": Aquí una vez seas atrapado, te convertirás en un pingüino "zombie" que va a ir dando caza al resto de su especie [Debatir esto]
- 
-


Para aumentar la experiencia y el tiempo de juego, hemos implementado varios modos de juego extra:
- Carrera: Los pingüinos harán una carrera por un escenario completamente nuevo diseñado para este modo de juego. La regla es simple, el primero que llega gana. 
- "Sumo":
-



## Movimientos y habilidades
La distintas velocidades de cada animal son el ajuste más importante del juego para poder conseguir una jugabilidad bien definida y agradable para los usuarios. Para compensar el desnivel de velocidades, hemos decidido que se ajuste mediante el juego de los usuarios cuando utilicen sus sprints y sus correspondientes habilidades.

Los sprints de cada animal son distintos:
- Pingüinos: Los pingüinos podrán aprovechar su velocidad instantánea o los desniveles causados por la nieve para poder tumbarse y aumentar su velocidad de movimiento de manera considerable mientras que este decae por la desaceleración. 
- Osos: Los osos podrán correr, pero la duración de su carrera va a estar limitada por una barra de aguante. De esta manera se balancea la ventaja de velocidad que este tiene frente a los adversarios. 




Cada animal dispondrá de una habilidad:

- Pingüinos: estos tendrán la habilidad de soltar cepos a sus espaldas, si uno de estos consigue colisionar con el oso, este se verá petrificado por unos instantes a modo de penalización. Este habilidad es permanente, pero tiene un tiempo de espera para poder volver a usarla. El correcto uso de esta habilidad es esencial para poder sobrevivir
- Osos: Los osos tendrán la habilidad de la visión Berserker. Para compensar el número de pingüinos y el tamaño del escenario, el oso cada cierto tiempo podrá ver la posición de todos los pingüinos. 

Con estas dos habilidades se busca un balanceo justo para las deficiencias de cada animal. Los pingüinos al tener un movimiento más lento, se encuentran en una clara desventaja a la hora de huir en sitios donde el deslizamiento sea casi nulo. 
Los osos podrán obtener la posición de sus presas con cierta frecuencia para aprovechar también los laberintos que puede encontrar en la ciudad y en las montañas y no hacer de su rol una misión imposible. 
## Mecánicas de juego


## Escenarios 

El juego tiene un escenario por cada modo disponible. Cada escenario ha sido desarrollado pensando en expandir la jugabilidad de su modo correspondiente. El trabajo de level design en este proyecto es muy importante, puesto que el número de jugadores y el tiempo aproximado de partida son decisivos a la hora de crear estos escenarios.

Para este proyecto hemos buscado varios paquetes de assets ya definidos para incorporarlos. El trabajo de búsqueda ha sido muy importante para conseguir encontrar los elementos que más se acerque a la estética que marcan los personajes.


Escenario Hunt: El escenario principal del modo hunt está dividido por cuadrantes. En cada cuadrante hay una área especial para sacar el rendimiento a las físicas y a las propias reglas del juego.

- Zona hielo: A la hora de concebir el juego esta era una zona fundamental. Gran parte de la esencia a la hora de definir las físicas de este juego, han sido orientadas al desarrollo de esta parte del escenario. El hielo juega con el deslizaminto y y la inercia para poder incidir mejor en la movimiento de los personajes. En este punto del juego se lleva a los jugadores.(Físicas)

Agujeros......

(Incluir fotos)



- Zona ciudad: Esta parte está inspiranda en la ciudad de Pyramiden, fue un asentamiento minero de carbón construido por los soviéticos. La función de este área es jugar con las posiciones de los edificios y utilizarlos como encondite. Esto sumado al sonido por proximidad del oso, se convierte en una combinación efectiva a la hora de recrear la sensación de estar escondido. El ambiente esta zona es un representación de cómo se encuentra el asentamiento minero en la actualidad, abandonado y con los edificios en declive.

Imagen del pequeño pueblo abandonado.
![imagen](https://user-images.githubusercontent.com/55508821/103051659-a117c580-4597-11eb-8291-0b3ff7270de1.png)

![imagen](https://user-images.githubusercontent.com/55508821/103051727-c6a4cf00-4597-11eb-9311-80a5813b2329.png)

(Incluir aqui las imágenes del escenario en esa zona)

Todos los edificios implementados en el mapa son material gratuito buscado a conciencia para crear una estética homogenea entre si. La premisa para esta zona del escenario es recrear un pequeño pueblo sumergido en la nieve. 








- Zona laberinto en las montañas: Esta zona se utiliza para intensificar la sensación de persecución utilizando como factor clave la sensación de agobio provocada por el pequeño espacio entre las laderas de las montañas y sus estrechos pasadizos. Es un sitio ideal para despistar entre varias a los osos debido al número de recorridos que hay. 

- Zona Bosque: 

- Zona Copo de nieve: 

Escenario Sumo:

Escenario Carreras:



## Interfaz


## Menús


## Flujo de juego
![Polo Hunt](https://user-images.githubusercontent.com/55508821/103471602-85c06d80-4d82-11eb-882c-96868767f85c.png)




## Estilo artístico

## Música

## Experiencia de usuario

Este juego tiene un gran desafío por esta parte. Conseguir la diversión del jugador es la clave para el éxito del producto y para ello tenemos unas cuantas reglas principales:
- El movimiento de los personajes debe ser atractivo a la hora de su manejo, pero con la premisa de que la torpeza de los movimientos se encuentren en un elaborado balance entre las físicas de los personajes y la influencia del escenario. 
- Los modos de juego deben de tener una complejidad muy ligera. Los modos de juego tienen las siguientes caracteristicas.....
- La temática es sin duda una de las claves de este juego. La misión principal de la temática y de sus personajes es conquistar al jugador y dotar al juego de una personalidad amable y graciosa. 
-

## Servidores y multijugador
Polar Hunt hace uso del framework de desarrollo Photon Unity Networking 2 (PUN2) y del servicio de servidores Photon Cloud. Los servidores permiten hasta 20 personas jugando de manera simultánea, con cross-play entre las plataformas disponibles. Los jugadores pueden crear salas públicas disponibles para el resto de usuarios o salas privadas accesibles mediante un código.
Los servidores de Photon Cloud tienen una latencia de 90-100 milisegundos de media. A pesar de ser unos valores aceptables para nuestro tipo de juego, Polar Hunt utiliza diferentes métodos para minimizar el impacto de la latencia. El networking de las posiciones de los personajes cuenta con predicción de movimientos e interpolación de los datos reales con los estimados. Cada jugador tiene autoridad sobre su propio personaje, asegurando así que la respuesta al input del sea rápida y que el usuario mantenga siempre el control sobre su personaje. También cuenta con comprobaciones de latencia y sincornización entre los jugadores a la hora de decidir eventos importantes como puede ser cazar a un pingüino. 
Polar Hunt tiene arquitectura cliente-servidor para facilitar la conexión entre los usuarios y evitar así las dificultades que les puede suponer sortear la NAT para conectarse entre ellos. Esto es esencial sobre todo para juegos de navegador y orientados a todos los públicos, ya que debe primar la accesibilidad.

##Tutorial

## Modelo de negocio
Nuestro modelo de negocio tiene como clave principal la fidelización de nuestros clientes. Vamos a ir asegurando su permanencia con una estrategia a dos años vista con continuas renovaciones del juego. 

La principal fuente de ingresos para este juego es la tienda que ofrece. El contenido de la tienda se renueva semanalmente con ofertas para aquellos jugadores que hayan superado los desafíos establecidos. Estos desafíos incitan al jugador a jugar de manera casi constante para que se vea beneficiado por la parte económica del ahorro en las skins.


Para conservar a nuestra audiencia vamos a ir actualizando el juego con una buena variedad de modos de juego siguiendo con la esencia de los principales. Cada x tiempo se lanzará un nuevo modo junto con un paquete de skins para los personajes del juego.

Modos de juego para el futuro:
-
-
-
-
-

Paquetes de Skins:
-
-
-
-
-


## Controles
Los controles del juego varian en función a cada plataforma, ya que en la versión de ordenador se puede jugar con el teclado y ratón, y también se podrá jugar con mando. Por la parte de dispositivos móviles solo vamos a ofrecer el control del juego a traves de la pantalla táctil.

Ordenador:
Teclador y ratón:

Consola:



Móvil:





