using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Sincronizacion info ataques, muertes y eso
public class MatchManager : MonoBehaviourPun
{
    #region VARIABLES
    public List<Transform> startPositions;
    public List<Transform> fishPositions;
    public GameObject penguinPrefab;
    public GameObject bearPrefab;

    public MatchInfo matchInfo;
    public MatchInfo matchInfoPrefab;
    #endregion


    #region UNITY CALLBACKS

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(this.matchInfoPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }
    void Start()
    {
        matchInfo = FindObjectOfType<MatchInfo>();
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
    [PunRPC]
    public void EatFish(int fishId)
    {
        matchInfo.DestroyFish(fishId);
    }

    public void InstantiatePlayers()
    {
        object isPenguin;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isPenguin", out isPenguin);
        if (isPenguin == null)
        {
            isPenguin = true;
        }
        if ((bool)isPenguin)
        {
            GameObject myPenguin = PhotonNetwork.Instantiate(this.penguinPrefab.name, startPositions[0].position, Quaternion.identity, 0);
            myPenguin.GetComponent<PenguinInputMultiplayer>().ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        }
        else
        {
            GameObject myBear = PhotonNetwork.Instantiate(this.bearPrefab.name, startPositions[0].position, Quaternion.identity, 0);
            myBear.GetComponent<BearInputMultiplayer>().ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }

}
