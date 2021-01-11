using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{

    public float kRigidez; // Constante de rigidez del muelle (N/m)
    public float length0; // Longitud natural del muelle (ahí la fuerza se anula)
    public float length; // Longitud del muelle en un momento dado
    public Vector3 pos; // Posición vertical del punto medio del muelle
    public Vector3 dir; // Vector dirección del muelle
    public float defaultSize = 2f; // Longitud natural de los cilindros en Unity (m)
    public Quaternion rotation; // Rotación del muelle
    public Node nodeA; // Primer extremo del muelle
    public Node nodeB; // Segundo extremo del muelle    
    public float dRotation; // Amortiguamiento debido a la rotación
    public float dDeformation; // Amortiguamiento de la deformación
    public bool tipoMuelle; // Variable que indica si es un muelle de flexión (true) o de tracción (false)

    // Método constructor del muelle
    public Spring(Node a, Node b, float k, bool tip, float dR, float dD)
    {
        // Asignamos los datos de entrada a los respectivos atributos del muelle
        this.nodeA = a;
        this.nodeB = b;
        this.kRigidez = k;
        this.tipoMuelle = tip;
        this.dRotation = dR;
        this.dDeformation = dD;
        // Calculamos la longitud inicial del muelle 
        this.length0 = Vector3.Magnitude(nodeA.pos - nodeB.pos);
        // Asignamos a la longitud instantánea la longitud inicial del muelle (ya que coincide con el momento inicial)
        this.length = length0;
        // Calculamos la posición del muelle
        this.pos = (nodeA.pos + nodeB.pos) / 2;
        // Calculamos el vector dirección del muelle
        this.dir = Vector3.Normalize(nodeA.pos - nodeB.pos);
        // Calculamos la rotación del muelle
        this.rotation = Quaternion.FromToRotation(Vector3.up, dir);

    } // Fin método constructor

    // Método para dibujar los gizmos de los muelles
    public void OnDrawGizmos()
    {
        // Si el muelle es de flexión (tipoMuelle = true)
        if (this.tipoMuelle == true)
        {
            // Establecemos el color del muelle en azul
            Gizmos.color = Color.blue;
        }
        // Si el muelle es de tracción (tipoMuelle = false)
        else
        {
            // Establecemos el color del muelle en rojo
            Gizmos.color = Color.red;
        }
        Gizmos.DrawLine(nodeA.pos, nodeB.pos);
    } // Fin método onDrawGizmos

} // Fin clase Spring
