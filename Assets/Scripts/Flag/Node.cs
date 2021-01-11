using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    // Declaramos los atributos de la clase Node
    public bool fixedNode; // Indica si es un nodo fijo o puede moverse
    public float mass; // Masa del nodo (kg)
    public Vector3 pos; // Posición del nodo
    public Vector3 force; // Fuerza que se aplica sobre el nodo
    public Vector3 vel; // Velocidad del nodo
    public float dAbsolute = 0.1f; // Factor de amortiguamiento del muelle absoluto

    // Método constructor del nodo
    public Node (Vector3 pos, float masa, bool fijo, float dA)
    {
        // Asignamos los datos de entrada a los respectivos atributos del nodo
        this.pos = pos;
        this.mass = masa;
        this.fixedNode = fijo;
        this.dAbsolute = dA;
        // Inicializamos la velocidad y la fuerza a cero
        this.vel = new Vector3(0, 0, 0);
        this.force = new Vector3(0, 0, 0);

    } // Fin método constructor

    // Método para dibujar los gizmos de los nodos
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.pos, 0.1f);
    }
    /*
     // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */

} // Fin clase Node
