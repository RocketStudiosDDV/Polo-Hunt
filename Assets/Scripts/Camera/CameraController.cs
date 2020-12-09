﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private PlayerControls _controls;
    public GameObject player;
    public GameObject reference; //la camara maneja a la referencia para tener los ejes bien colocados
    public Vector3 distance; //distancia que mantioene la camar con el jugador


    // Start is called before the first frame update
    void Start()
    {
        distance = transform.position - player.transform.position; //Devuelve el vector que va del jugador a la camara
    }

    //Se asegura de que el jugador se movio
    private void LateUpdate()
    {
        transform.position = player.transform.position + distance;
        transform.LookAt(player.transform.position);
    }

   
}
