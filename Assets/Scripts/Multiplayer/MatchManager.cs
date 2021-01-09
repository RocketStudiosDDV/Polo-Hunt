using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Clase presente en la escena, almacena StartPositions y FishPositions
/// </summary>
public class MatchManager : MonoBehaviourPun
{
    #region VARIABLES
    // Ajustes para el inspector
    public List<Transform> startPositions;  // Posiciones de inicio de los jugadores
    public List<Transform> fishPositions;   // Posiciones para instanciar pescados

    // Datos
    public GameMode gameMode;  // Modo de juego

    // Prefabs
    public GameObject penguinPrefab;    // Prefab del pingüino modo caza
    public GameObject penguinRacePrefab;    // Prefab del pingüino modo carrera
    public GameObject bearPrefab;   // Prefab del oso
    public GameObject matchInfoPrefab;   // Prefab matchInfo

    // Referencias
    public MatchInfo matchInfo;
    #endregion

    #region UNITY CALLBACKS
    /// <summary>
    /// El MasterClient instancia un matchInfo para todos los jugadores que almacena la información de partida y administra los eventos
    /// Todos los jugadores leen el modo de juego de las customProperties de la room
    /// </summary>
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(this.matchInfoPrefab.name, Vector3.zero, Quaternion.identity);
        }
        
        object customProperty;
        int gameModeIndex = 0;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out customProperty))
            gameModeIndex = (int)customProperty;
        switch (gameModeIndex)
        {
            case 0:
                gameMode = GameMode.Hunt;
                break;
            case 1:
                gameMode = GameMode.Race;
                break;
        }
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Instancia cada cliente su personaje dependiendo de si es pingüino u oso
    /// </summary>
    public void InstantiatePlayers()
    {
        object isPenguin;
        object playerIdProperty;
        int playerId = 0;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isPenguin", out isPenguin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("playerId", out playerIdProperty);
        if (playerIdProperty != null)
            playerId = (int) playerIdProperty;
        if (gameMode == GameMode.Hunt)
        {
            if (isPenguin == null)
            {
                isPenguin = true;
            }
            if ((bool)isPenguin)
            {
                GameObject myPenguin = PhotonNetwork.Instantiate(this.penguinPrefab.name, startPositions[playerId].position, Quaternion.identity, 0);
                myPenguin.GetComponent<PenguinInputMultiplayer>().ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            }
            else
            {
                GameObject myBear = PhotonNetwork.Instantiate(this.bearPrefab.name, startPositions[playerId].position, Quaternion.identity, 0);
                myBear.GetComponent<BearInputMultiplayer>().ownerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            }
        } else
        {
            GameObject myPenguin = PhotonNetwork.Instantiate(this.penguinRacePrefab.name, startPositions[playerId].position, Quaternion.identity, 0);
        }    
    }

    public void ReturnToGameSelectionMenu()
    {
        FindObjectOfType<ConectionManagerInGame>().ReturnToGameSelectionMenu();
    }
    #endregion
}
