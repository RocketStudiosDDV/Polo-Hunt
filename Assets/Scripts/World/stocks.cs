﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stocks : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.name == "Bear")
        {
            Destroy(gameObject, 0.05f); //Se destruye dos segs después de la colisión
        }

    }
}
