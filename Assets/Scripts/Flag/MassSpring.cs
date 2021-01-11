using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class MassSpring : MonoBehaviour
{
    // Booleano para pausar o arrancar la animación
    public bool paused;

    // Booleano para activar o desactivar los gizmos
    public bool gizmosActivos;

    // Vector3 que hace referencia a la gravedad
    public Vector3 g = new Vector3(0f, 9.8f, 0f);
    // Vector que hace referencia al viento, no lo inicializamos a ningún valor, así, al arrancar la animación veremos como
    // la bandera cae y podremos jugar con los valores del viento a nuestro gusto para poder ver qué cambios se producen
    public Vector3 wind;
    // Variable para controlar la rotación de la dirección del viento
    public float windDirection = 5f;
    // Variable para controlar la intensidad del viento
    public float windSpeed = 2;
    // Variable para calcular desviaciones y ráfagas de viento aleatorias
    public float randomWind = 30f;

    // Definimos la propiedad de masa de los nodos
    public float masa = 1.5f;

    // Definimos la propiedad/constante de rigidez de los muelles de flexión
    public float kFlexion = 150.0f;
    // Definimos la propiedad/constante de rigidez de los muelles de tracción
    public float kTraccion = 1100.0f;

    // Definimos el amortiguamiento de los muelles
    public float dRotation = 0.8f; // Amortiguamiento debido a la rotación
    public float dDeformation = 1.5f; // Amortiguamiento de la deformación

    // Definimos el amortiguamiento de los nodos    
    public float dAbsolute = 0.6f; // Factor de amortiguamiento del muelle absoluto

    // Lista enumerada con los dos métodos de integración requeridos
    public enum Integration
    {
        SymplecticEuler = 1,
        ExplicitEuler = 0,        
    }
    // Método de integración con el que vamos a calcular la animación
    public Integration integrationMethod;
    // Paso de integración (es un tiempo)
    public float h = 0.01f;

    // Creamos la lista de nodos
    List<Node> listOfNodes;
    // Creamos la lista de muelles
    List<Spring> listOfSprings;
    // Creamos una lista para las aristas
    public List<Edge> listOfAristas;
    // Creamos una lista para los fixer
    public List<Fixer> listOfFixer;

    // Método start
    // Start is called before the first frame update
    void Start()
    {
        // Código empleado para acceder al mallado basado en triángulos de la tela
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        // Accedemos a los vértices del mallado y los almacenamos en un array de Vector3
        Vector3[] vertices = mesh.vertices;
        // Accedemos a los triángulos y los almacenamos en una array de int
        int[] triangles = mesh.triangles;

        // Inicializamos la lista de nodos con tamaño del número de vértices que tenga el mallado
        listOfNodes = new List<Node>(vertices.Length);
        // Inicializamos la lista de muelles
        listOfSprings = new List<Spring>();
        // Inicializamos la lista de aristas
        listOfAristas = new List<Edge>();

        // Establecemos la variable paused a false para que inicialmente la animación no está pausada
        paused = false;
        // Establecemos la variable gizmosActivos a true para que inicialmente los gizmos estén activados
        gizmosActivos = true;

        // Añadimos tantos nodos como vértices haya a la lista de nodos, para ello,
        // lo hacemos tantas veces como iteraciones realicemos hasta recorrer el array de vértices
        foreach (Vector3 vector in vertices)
        {
            // Llamamos al método detectarFixer para comprobar si el nodo está dentro de la zona del espacio ocupada por un fixer 
            if (detectarFixer(transform.TransformPoint(vector), listOfFixer))
            {
                // Añadimos el nodo llamando al constructor de Node, pasándole al mismo la posición del vértice, la masa declarada anteriormente y true ya que es un nodo fijo            
                listOfNodes.Add(new Node(transform.TransformPoint(vector), masa, true, dAbsolute));
            }
            else
            {
                // Añadimos el nodo llamando al constructor de Node, pasándole al mismo la posición del vértice, la masa declarada anteriormente y false ya que no es un nodo fijo            
                listOfNodes.Add(new Node(transform.TransformPoint(vector), masa, false, dAbsolute));
            }
        }

        // Recorremos el array de triángulos de 3 en 3 para así avanzar de un triángulo a otro (ya que el array está compuesto por enteros, cada 3 enteros componen 1 triángulo)
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Creamos las tres posibles aristas (ya que un triángulo está formado por tres aristas) con cada grupo de tres vértices, de los cuales los dos primeros
            // harán referencia a la arista y se usarán para crear los muelles de tracción; y el tercero nos servirá para crear posteriormente los muelles de flexión
            listOfAristas.Add(new Edge(triangles[i], triangles[i + 1], triangles[i + 2]));
            listOfAristas.Add(new Edge(triangles[i + 1], triangles[i + 2], triangles[i]));
            listOfAristas.Add(new Edge(triangles[i + 2], triangles[i], triangles[i + 1]));
        }
        // Ordenamos la lista de aristas según el método ordenarArista (de forma ascendente, de menor a mayor según los vértices A y B)
        listOfAristas.Sort(ordenarArista);

        // Generamos una arista para comparar las aristas de la lista, le asignamos le valor (-1,-1,-1) para que en la primera iteración entre en la rama del else y se cree un muelle de tracción
        Edge arista = new Edge(-1, -1, -1);
        // Recorremos la lista de aristas
        foreach (Edge ar in listOfAristas)
        {
            // Comparamos los vértices A y B de la arista actual (ar) con la anterior (arista), si coinciden, creamos un muelle de flexión
            if (ar.vertexA == arista.vertexA && ar.vertexB == arista.vertexB)
            {
                // Creamos y añadimos a la lista de muelles un nuevo muelle de flexión, cuyos nodos serán los situados en las posiciones de los
                // vértices C de ambas aristas (ar y arista), y cuya constante de rigidez será k (constante de rigidez para los muelles de flexión)
                // El último parámetro, true, hace referencia a que es un muelle de flexión
                listOfSprings.Add(new Spring(listOfNodes[ar.vertexC], listOfNodes[arista.vertexC], kFlexion, true, dRotation, dDeformation));
            }
            // Si no coincide, creamos un muelle de tracción
            else
            {
                // Creamos y añadimos a la lista de muelles un nuevo muelle de tracción. Sus nodos serán los situados en las posiciones de los
                // vértices A y B de la arista ar (como vemos, el muelle coincide con la arista como tal, es decir, los muelles de tracción se colocan
                // en la posición de las distintas aristas en las que se descomponen los triángulos del mallado)
                // Su constante de rigidez será kK (constante de rigidez para los muelles de tracción)
                // El último parámetro, false, hace referencia a que no es un muelle de flexión, sino de tracción
                listOfSprings.Add(new Spring(listOfNodes[ar.vertexA], listOfNodes[ar.vertexB], kTraccion, false, dRotation, dDeformation));
            }
            // Actualizamos la arista para seguir comparándolas
            arista = ar;
        }

    } // Fin método Start

    // Método update
    // Update is called once per frame
    void Update()
    {

        //Accedemos al mallado basado en triángulos de la tela
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        // Creamos una lista auxiliar para guardar las nuevas posiciones (actualizadas en función del movimiento de los nodos) de los vértices
        // La lista de dichos vértices es de tipo Vector3 ya que las posiciones de los nodos se almacenan en este tipo de dato
        // para guardar las nuevas posiciones en local(transform.InverseTransformPoint)
        List<Vector3> verticesnuevos = new List<Vector3>();

        // Recorremos la lista de nodos
        foreach (Node nodo in listOfNodes)
        {
            // Añadimos a la lista un nuevo vértice según la posición del nodo pero ajustada (mediante transform.InverseTransformPoint) a la tela
            verticesnuevos.Add(transform.InverseTransformPoint(nodo.pos));
        }
        // Actualizamos la lista original de vértices del mallado asignándole la lista auxiliar
        mesh.SetVertices(verticesnuevos);

        // Detectamos si se ha pulsado la tecla P
        if (Input.GetKeyUp(KeyCode.P))
        {
            // La tecla P hace de "toggle" para pausar o arrancar la animación
            paused = !paused;
        }

    } // Fin método Update

    private void FixedUpdate()
    {

        // Si está pausada la animación, no hacemos nada y regresamos
        if (paused)
            return;

        // Llamamos al método controlar viento
        controlarViento();

        // Mediante un switch, según el método de integración elegido en el inspector de Unity, llamamos a una función u otra
        switch (integrationMethod)
        {

            // Si el método de integración escogido es el Simpléctico, llamamos a su correspondiente función
            case Integration.SymplecticEuler:
                integrateSymplecticEuler();
                break;

            // Si el método de integración escogido es el Explícito, llamamos a su correspondiente función
            case Integration.ExplicitEuler:
                integrateExplicitEuler();
                break;                      

            // Si no coincide con ninguno, devolvemos un mensaje de error
            default:
                print("ERROR METODO INTEGRACION DESCONOCIDO");
                break;
        }

        // Recorremos la lista de muelles
        foreach (Spring spring in listOfSprings)
        {
            // Actualizamos las propiedades de los muelles
            // Vector dirección del muelle, apunta de B a A    
            spring.dir = spring.nodeA.pos - spring.nodeB.pos;
            // Nueva longitud del muelle 
            spring.length = spring.dir.magnitude;
            // Normalizamos el vector que almacena la orientación del muelle
            spring.dir = Vector3.Normalize(spring.dir);
            // Posición del punto medio del muelle: media aritmética de las posiciones de los dos nodos
            spring.pos = (spring.nodeA.pos + spring.nodeB.pos) / 2f;
            // Orientamos correctamente el muelle según el vector dir
            spring.rotation = Quaternion.FromToRotation(Vector3.up, spring.dir);
        }

    } // Fin método FixedUpdate

    // Método de integración de Euler Explícito
    void integrateExplicitEuler()
    {
        // Recorremos la lista de nodos para aplicar las fuerzas a cada uno de ellos
        foreach (Node node in listOfNodes)
        {
            // Si el nodo no es fijo
            if (!node.fixedNode)
            {
                // Actualizamos la posición
                node.pos += h * node.vel; // r_(n+1) = r_n + h * v_n
                // Aplicamos la fuerza de la gravedad
                node.force = -node.mass * g;
                // Aplicamos el viento
                node.force += wind;
                // Aplicamos pequeñas desviaciones aleatorias en la dirección perpendicular a la dirección nominal del viento
                // calculando el vector normal a la dirección del viento y al vector up (paralelo al eje y), normalizándolo y
                // multiplicándolo por un Random cuyo rango se puede modificar desde el inspector de Unity
                node.force += Vector3.Cross(wind, Vector3.up).normalized  * Random.Range(-randomWind, randomWind);
                // Aplicamos el amortiguamiento a los nodos
                applyDampingNode(node);
            }
        }

        // Recorremos la lista de muelles para aplicar las fuerzas a cada uno de ellos
        foreach (Spring spring in listOfSprings)
        {
            // Añadimos la fuerza elástica a los dos nodos que conectan con el muelle
            spring.nodeA.force += -spring.kRigidez * (spring.length - spring.length0) * spring.dir;
            spring.nodeB.force += spring.kRigidez * (spring.length - spring.length0) * spring.dir;
            // Aplicamos el amortiguamiento a los muelles
            applyDampingSpring(spring);

        }

        // Calculamos la nueva velocidad de cada uno de los nodos
        foreach (Node node in listOfNodes)
        {
            // Si el nodo no es fijo
            if (!node.fixedNode)
            {
                node.vel += h * node.force / node.mass; // v_(n+1) = v_n + h F_n / m
            }
        }

    } // Fin método integrateExplicitEuler

    // Método de integración de Euler Simpléctico
    void integrateSymplecticEuler()
    {

        // Recorremos la lista de nodos para aplicar las fuerzas a cada uno de ellos
        foreach (Node node in listOfNodes)
        {
            // No hace falta comprobar si el nodo es o no fijo ya que aquí no accedemos a su posición y velocidad
            // Aplicamos la fuerza de la gravedad
            node.force = -node.mass * g;
            // Aplicamos la fuerza del viento (definida en el Inspector de Unity)
            // Aplicamos el viento
            node.force += wind;
            // Aplicamos pequeñas desviaciones aleatorias en la dirección perpendicular a la dirección nominal del viento
            // calculando el vector normal a la dirección del viento y al vector up (paralelo al eje y), normalizándolo y
            // multiplicándolo por un Random cuyo rango se puede modificar desde el inspector de Unity
            node.force += Vector3.Cross(wind, Vector3.up).normalized  * Random.Range(-randomWind, randomWind);
            // Aplicamos el amortiguamiento a los nodos
            applyDampingNode(node);
        }

        // Recorremos la lista de muelles para aplicar las fuerzas a cada uno de ellos
        foreach (Spring spring in listOfSprings)
        {
            // Añadimos la fuerza elástica a los dos nodos que conectan con el muelle
            spring.nodeA.force += -spring.kRigidez * (spring.length - spring.length0) * spring.dir;
            spring.nodeB.force += spring.kRigidez * (spring.length - spring.length0) * spring.dir;
            // Aplicamos el amortiguamiento a los muelles
            applyDampingSpring(spring);
        }

        // Calculamos la nueva velocidad y la nueva posición de cada uno de los nodos
        foreach (Node node in listOfNodes)
        {
            // Si el nodo no es fijo
            if (!node.fixedNode)
            {
                // Calculamos la nueva velocidad
                node.vel += h * node.force / node.mass; // v_(n+1) = v_n + h F_n / m
                // Actualizamos la posición
                node.pos += h * node.vel; // r_(n+1) = r_n + h * v_n
            }
        }

    } // Fin método integrateSymplecticEuler

    // Método para aplicar el amortiguamiento debido a la rotación y de deformación de los muelles
    public void applyDampingSpring(Spring spring)
    {

        // Frenamos la rotación y la deformación del conjunto de todos los muelles añadimendo las siguientes fuerzas a los nodos que los componen
        spring.nodeA.force += (-spring.dRotation * (spring.nodeA.vel - spring.nodeB.vel));
        spring.nodeA.force += (-spring.dDeformation * Vector3.Dot(spring.nodeA.vel - spring.nodeB.vel, spring.dir) * spring.dir);

        spring.nodeB.force += (-spring.dRotation * (spring.nodeB.vel - spring.nodeA.vel));
        spring.nodeB.force += (-spring.dDeformation * Vector3.Dot(spring.nodeB.vel - spring.nodeA.vel, spring.dir) * spring.dir);

    } // Fin método applyDampingSpring 



    // Método para aplicar el amortiguamiento del muelle absoluto a los nodos
    public void applyDampingNode(Node node)
    {

        // Frenamos el movimineto absoluto de los nodos añadiendole la siguiente fuerza
        node.force += -node.dAbsolute * node.vel;

    } // Fin método applyDampingNode

    // Método para controlar la dirección y fuerza del viento mediante las teclas de dirección
    public void controlarViento()
    {
        
        // La tecla izquierda rota hacia la izquierda la dirección del viento
        if (Input.GetKey(KeyCode.J))
        {
            wind = Quaternion.Euler(0, -windDirection * 0.08f, 0) * wind;
        }
        // La tecla derecha rota hacia la derecha la dirección del viento
        if (Input.GetKey(KeyCode.L))
        {
            wind = Quaternion.Euler(0, windDirection * 0.08f, 0) * wind;
        }
        // La tecla superior aumenta la fuerza del viento
        if (Input.GetKey(KeyCode.I))
        {
            if (wind.magnitude == 0) { wind += new Vector3(0.1f, 0, 0); }
            wind += wind * 0.01f * windSpeed;
        }
        // La tecla inferior disminuye la fuerza del viento
        if (Input.GetKey(KeyCode.K))
        {
            wind -= wind * 0.01f * windSpeed;
        }

    } // Fin método controlar viento


    // Método para detectar si los nodos coinciden con el fixer y de esta forma declararlos como fijos o móviles
    bool detectarFixer(Vector3 pos, List<Fixer> listOfFixer)
    {

        // Recorremos la lista de los fixer
        foreach (Fixer fix in listOfFixer)
        {
            
            Collider m_Collider = fix.GetComponent<Collider>();

            if (m_Collider.bounds.Contains(pos))
            {
                return true;
            }

        }
        // En cualquier otro caso, false
        return false;

    } // Fin método detectarFixer



    // Método par aordenar las aristas de la lista de aristas
    int ordenarArista(Edge a, Edge b)
    {

        // Devolvemos -1 si la arista "a" está más a la izquierda (es menor) que la arista "b",
        // 1 en el caso contrario y 0 si son iguales
        // En primer lugar, comparamos los vértices A de ambas aristas
        // Si el vértice A de a está más a la izquierda que el de b
        if (a.vertexA < b.vertexA)
        {
            // Devolvemos -1
            return -1;
        }
        // Si el vértice A de a está más a la derecha que el de b
        else if (a.vertexA > b.vertexA)
        {
            // Devolvemos 1
            return 1;
        }
        // Si el vértice A de ambas están en la misma posición, comparamos los vértices B
        else
        {
            // Si el vértice B de a está más a la izquierda que el de b
            if (a.vertexB < b.vertexB)
            {
                // Devolvemos -1
                return -1;
            }
            // Si el vértice B de a está más a la derecha que el de b
            else if (a.vertexB > b.vertexB)
            {
                // Devolvemos 1
                return 1;
            }
            // Si el vértice B de ambas están en la misma posición, devolvemos 0 ya que ambas aristas son iguales
            else
            {
                // Devolvemos 0
                return 0;
            }
        }

    } // Fin método ordenarArista


    // Método para dibujar los gizmos
    public void OnDrawGizmos()
    {
        // Si los gizmos están activados
        if (gizmosActivos == true)
        {
            // Recorremos la lista de nodos
            foreach (Node nodo in listOfNodes)
            {
                // Dibujamos los gizmos de los mismos
                nodo.OnDrawGizmos();
            }
            // Recorremos la lista de muelles
            foreach (Spring spring in listOfSprings)
            {
                // Dibujamos los gizmos de los mismos
                spring.OnDrawGizmos();
            }
            // Dibujamos el viento
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(0, 0, 0), wind);
            /*
            // Dibujamos las desviaciones aleatorias perpendiculares a la dirección del viento
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(0, 0, 0), Vector3.Cross(wind, Vector3.up).normalized * Random.Range(-randomWind, randomWind));
            */
        }
    }


} // Fin clase MassSpring
