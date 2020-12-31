using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StocksMultiplayer : MonoBehaviourPun
{
    private MatchManager matchManager;

    private void Awake()
    {
        matchManager = FindObjectOfType<MatchManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Bear")
        {
            if (!PhotonNetwork.IsConnected)
            {
                Destroy(gameObject, 0.05f); //Se destruye dos segs después de la colisión
            } else
            {
                collision.gameObject.GetComponent<BearInputMultiplayer>().Stun();
                if (GetComponent<PhotonView>().IsMine)
                    PhotonNetwork.Destroy(gameObject);
            }
        }

    }
}
