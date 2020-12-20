using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    #region PUBLIC VARIABLES
    public Transform startPosition;
    public GameObject penguinPrefab;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(this.penguinPrefab.name, startPosition.position, Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
