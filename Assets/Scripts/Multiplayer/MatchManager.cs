﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Sincronizacion info ataques, muertes y eso
public class MatchManager : MonoBehaviour
{
    #region PUBLIC VARIABLES

    public List<Transform> startPositions;
    public GameObject penguinPrefab;
    public GameObject bearPrefab;

    #endregion


    #region UNITY CALLBACKS
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    //VER SI METERLO O NO
    public void KillPenguin()
    {

    }

    //MIRAR SI SE PUEDE SIONCRONIZAR SOLO CON LOS CAMBIOS PHOTOVIEW
    public void FallPlatform()
    {

    }

    //que cuando un pinguino se come un pescado, desaparezca para todos los pinguinos
    public void EatFish()
    {

    }

    public void StartMatch()
    {
        object isPenguin;
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isPenguin", out isPenguin) + " eo");
        if (isPenguin == null)
        {
            Debug.Log("NULOOOOOOOOOO");
            isPenguin = true;
        }
        if ((bool)isPenguin)
        {
            PhotonNetwork.Instantiate(this.penguinPrefab.name, startPositions[0].position, Quaternion.identity, 0);
        }
        else
        {
            PhotonNetwork.Instantiate(this.bearPrefab.name, startPositions[0].position, Quaternion.identity, 0);
        }
    }

}
