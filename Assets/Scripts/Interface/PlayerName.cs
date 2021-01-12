using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    private Transform nameHUD;
    //private Transform text;

    private void Awake()
    {
        nameHUD = FindObjectOfType<Canvas>().GetComponent<Transform>();        
    }
    void Start()
    {

        Debug.Log("HOLA ");

        /*if (GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        */
        Debug.Log("MI NOMBRE ES " + GetComponent<PhotonView>().Owner.NickName);
        //nameHUD.GetComponent<Text>().text = GetComponent<PhotonView>().Owner.NickName;
        nameHUD.Find("Text").GetComponent<Text>().text = GetComponent<PhotonView>().Owner.NickName;


        //text.Find("Text").GetComponent<Text>().text = GetComponent<PhotonView>().Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
