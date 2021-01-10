using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMultiplayer : MonoBehaviour
{
    public int id;  // Identificador para sincronización online
    public MatchInfo matchInfo; // Referencia a MatchInfo para eliminarse en los otros clientes

    private void Awake()
    {
        matchInfo = FindObjectOfType<MatchInfo>();   
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Bear")
        {
            if (matchInfo != null)
            {
                Destroy(gameObject);
                object parameter = id;
                matchInfo.GetComponent<PhotonView>().RPC("DestroyFish", RpcTarget.All, parameter);
            }
            if (!PhotonNetwork.IsConnected)
            {
                Destroy(gameObject); //Se destruye dos segs después de la colisión
            }
        }

        if (collision.gameObject.tag == "Penguin")
        {
            if (matchInfo != null)
            {
                Destroy(gameObject);
                object parameter = id;
                matchInfo.GetComponent<PhotonView>().RPC("DestroyFish", RpcTarget.All, parameter);
            }
            if (!PhotonNetwork.IsConnected)
            {
                Destroy(gameObject); //Se destruye dos segs después de la colisión
            }
        }

        if (collision.gameObject.tag == "RunnerCollider")
        {
            if (matchInfo != null)
            {
                Destroy(gameObject);
                object parameter = id;
                matchInfo.GetComponent<PhotonView>().RPC("DestroyFish", RpcTarget.All, parameter);
            }
            if (!PhotonNetwork.IsConnected)
            {
                Destroy(gameObject); //Se destruye dos segs después de la colisión
            }
        }

    }
}
