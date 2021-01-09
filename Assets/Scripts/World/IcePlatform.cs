using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script de plataforma de hielo
/// Online: se destruye si la pisa un oso y se comunica con matchManager para que la destruya en los demás clientes
/// Se identifica entre los clientes con el ID que se debe asignar manualmente en el inspector (y debe ser único) a cada plataforma.
/// Offline: se destruye si la pisa un oso
/// </summary>
public class IcePlatform : MonoBehaviour
{
    /// <summary>
    /// Debe ser un ID (int) único asignado a cada plataforma manualmente
    /// Sirve para identificar la plataforma en todos los clientes
    /// </summary>
    public int platformId;

    /// <summary>
    /// Cuando entra en colisión con un oso, destruye la plataforma.
    /// Si es online, se comunica con MatchManager para que la destruya en los demás clientes
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsConnected)  // Versión online
        {
            if (collision.gameObject.tag == "Bear")
            {
                if (collision.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    Destroy(gameObject, 0.05f);
                    object parameter = platformId;
                    FindObjectOfType<MatchInfo>().GetComponent<PhotonView>().RPC("DestroyPlatform", RpcTarget.All, parameter);
                }
            }
        }
        else   // Versión offline
        {
            if (collision.gameObject.tag == "Bear")
            {
                Destroy(gameObject, 0.05f);
            }
        }               
    }
}
