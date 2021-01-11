using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{

    // Declaramos los atributos de la clase Edge, 3 atributos vértices ya que las aristas se crean a partir de
    // los triángulos del mallado (los cuáles están formados por 3 vértices y 3 aristas)
    // Los dos primeros vértices harán referencia a la arista como tal y se usarán para crear los muelles de tracción 
    public int vertexA; 
    public int vertexB;
    // El tercer vértice nos servirá para crear posteriormente los muelles de flexión
    public int vertexC;

    // Método constructor de la arista
    public Edge(int vertexA, int vertexB, int vertexC)
    {
        // Creamos una lista auxiliar para ordenar los dos primeros vértices (ya que son los que se corresponden con la arista como tal)
        List<int> Ordenar = new List<int>();
        // Añadimos dichos vértices
        Ordenar.Add(vertexA);
        Ordenar.Add(vertexB);
        // Ordenamos la lista para así después crear correctamente los muelles
        Ordenar.Sort();
        // Asignamos los valores de la lista ordenada a los respectivos vértices
        this.vertexA = Ordenar[0];
        this.vertexB = Ordenar[1];
        // Asignamos el valor del último vértice
        this.vertexC = vertexC;

    } // Fin método constructor

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

} // Fin clase Edge
