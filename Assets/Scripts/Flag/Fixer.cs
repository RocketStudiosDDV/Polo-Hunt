using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixer : MonoBehaviour
{

    // Posición del fixer
    public Vector3 pos;
    // Tamaño del fixer
    public Vector3 tam;

    //Método start
    // Start is called before the first frame update
    void Start()
    {
        // Accedemos al mallado del objeto
        MeshRenderer mesh = this.GetComponent<MeshRenderer>();
        // Lo hacemos invisible
        mesh.enabled = false;
        // Establecemos el valor de la posición "pos" a partir de la transformada position del gameObject
        pos = transform.position;
        // Establecemos el valor del tamaño "tam" a partir de la transformada localScale del gameObject
        tam = transform.localScale;

    } // Fin método start

    /*
    // Update is called once per frame
    void Update()
    {

    }
    */
}
