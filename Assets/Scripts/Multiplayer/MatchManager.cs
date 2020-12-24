using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Sincronizacion info ataques, muertes y eso
public class MatchManager : MonoBehaviour
{
    #region PUBLIC VARIABLES

    public Transform startPosition;
    public GameObject penguinPrefab;

    #endregion


    #region UNITY CALLBACKS
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(this.penguinPrefab.name, startPosition.position, Quaternion.identity, 0);
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

}
