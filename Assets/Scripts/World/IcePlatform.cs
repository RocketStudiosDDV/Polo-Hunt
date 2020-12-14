using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Bear")
        {
            Destroy(gameObject, 0.05f); //Se destruye dos segs después de la colisión
        }
        
    }
}
