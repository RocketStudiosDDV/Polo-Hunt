using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fish : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Bear")
        {
            Destroy(gameObject); //Se destruye dos segs después de la colisión
        }

        if (collision.gameObject.tag == "Penguin")
        {
            Destroy(gameObject); //Se destruye dos segs después de la colisión
        }

    }
}
