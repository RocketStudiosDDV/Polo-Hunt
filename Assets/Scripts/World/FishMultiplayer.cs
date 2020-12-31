using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMultiplayer : MonoBehaviour
{
    public int id;  // Identificador para sincronización online
    public MatchManager matchManager; // Referencia a MatchInfo para eliminarse en los otros clientes

    private void Awake()
    {
        matchManager = FindObjectOfType<MatchManager>();   
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Bear")
        {
            if (matchManager != null)
            {
                matchManager.EatFish(id);
            }
            if (!PhotonNetwork.IsConnected)
            {
                Destroy(gameObject); //Se destruye dos segs después de la colisión
            }
        }

        if (collision.gameObject.tag == "Penguin")
        {
            if (matchManager != null)
            {
                matchManager.EatFish(id);
            }
            if (!PhotonNetwork.IsConnected)
            {
                Destroy(gameObject); //Se destruye dos segs después de la colisión
            }
        }

    }
}
