using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinguPrueba : MonoBehaviour
{

    private Rigidbody pinga;
    private bool caer = false;
    private bool tirar = false;
    private float _timeFall;


    void Start()
    {
        pinga = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Penguin") //Si choca con un pescao
        {
            _timeFall = Time.fixedTime + 3;
        }

    }

    private void FixedUpdate()
    {
        if (pinga.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
        {
            if (Time.fixedTime > _timeFall)
            {
                pinga.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}
