using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// Administrador de conexión in-game
/// Permite salir de la partida, obtener la lista de jugadores y otras funciones
/// Versión de ConnectionManager simplificada
/// </summary>
public class ConectionManagerInGame : MonoBehaviourPunCallbacks, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks
{
    #region VARIABLES
    // PUBLICAS
    public LogWriter logWriter;

    /*
    public GameObject LobbyPanel;
    public GameObject ConnectPanel;
    public GameObject RoomPanel;
    public GameObject ChooseTypePanel;
    public GameObject OnlineOfflinePanel;
    public GameObject CreateRoomPanel;
    public GameObject RoomValuesPanel;
    public GameObject PlayersValuesPanel;
    public GameObject FinalRoomPanel;
    public GameObject FinalRoomPanel2;

    // PRIVADAS
    HashSet<int> roomCodes = new HashSet<int>();
    */
    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Desconecta el cliente del servidor de Photon
    /// Callback OnDisconnected
    /// </summary>
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// Devuelve el string del nombre de la sala
    /// null si no está en sala
    /// </summary>
    /// <returns></returns>
    public string GetRoomName()
    {
        if (PhotonNetwork.InRoom)
        {
            return PhotonNetwork.CurrentRoom.Name;
        }
        return null;
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    /// <summary>
    /// Devuelve una lista de String con los NickNames de los jugadores presentes en la sala.
    /// </summary>
    /// <returns></returns>
    public List<string> GetPlayersList()
    {
        List<string> playersList = new List<string>();
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
        foreach(Player player in players.Values)
        {
            playersList.Add(player.NickName);
        }
        return playersList;
    }

    /// <summary>
    /// Echa al jugador pasado como parámetro de la sala.
    /// Sólo lo puede echar el Master Client (host).
    /// Se debe pasar su NickName (string).
    /// Se obtienen los nicknames con GetPlayersList()
    /// </summary>
    /// <param name="playerNickName"></param>
    public void KickPlayer(string playerNickName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            foreach (Player player in players.Values)
            {
                if (player.NickName.Equals(playerNickName))
                {
                    PhotonNetwork.CloseConnection(player);
                }
            }
        }
    }

    /// <summary>
    /// Vuelven todos los clientes a la sala de espera pre-partida
    /// </summary>
    /// <param name="cause"></param>
    public void ReturnToGameSelectionMenu(int timeToReturn = 0)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToReturn == 0)
            {
                ExitGames.Client.Photon.Hashtable newCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                newCustomRoomProperties["hasStarted"] = false;
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomRoomProperties);
                PhotonNetwork.LoadLevel("MultiplayerTestScene");
            } else
            {
                Invoke(nameof(ReturnToGameSelectionMenuAux), timeToReturn);
            }
        }
    }

    private void ReturnToGameSelectionMenuAux()
    {
        ExitGames.Client.Photon.Hashtable newCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        newCustomRoomProperties["hasStarted"] = false;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomRoomProperties);
        PhotonNetwork.LoadLevel("MultiplayerTestScene");
    }
    #endregion

    #region PUN CALLBACKS
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        if (logWriter != null)
            logWriter.Write("Se ha desconectado por la causa: " + cause.ToString());
        /*
        ConnectPanel.SetActive(true);
        ChooseTypePanel.SetActive(false); 
        LobbyPanel.SetActive(false);
        CreateRoomPanel.SetActive(false); 
        RoomValuesPanel.SetActive(false);
        RoomPanel.SetActive(false);
        */
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (logWriter != null)
            logWriter.Write("Unido a la sala " + GetRoomName() + " - nº players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers);

        //FinalRoom(PhotonNetwork.CurrentRoom.PlayerCount-1);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (logWriter != null)
            logWriter.Write("Sala abandonada");
        SceneManager.LoadScene("MultiplayerTestScene");
        /*
        ChooseTypePanel.SetActive(true);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        ConnectPanel.SetActive(false);
        */
    }

    /// <summary>
    /// Si se va el host, termina la partida
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (logWriter != null)
            logWriter.Write("el host se fue");
        // TODO - mostrar aviso de que el host se fue y que se va a salir de la partida en timeToLeave segundos
        MatchInfo matchInfo = FindObjectOfType<MatchInfo>();
        if (matchInfo != null)
        {
            matchInfo.ShowAlertHUD(5, "El host se ha desconectado.\nSaliendo de partida...");
        }
        float timeToLeave = 5f;
        Invoke(nameof(LeaveRoom), timeToLeave);
    }
    #endregion

    public void CompruebaUser()
    {
        
    }
    /*
    public void MultiplayerButton()
    {     
        OnlineOfflinePanel.SetActive(false); 
        ConnectPanel.SetActive(true);
    }

    public void ConnectButton()
    {     
        ConnectPanel.SetActive(false); 
        ChooseTypePanel.SetActive(true);
    }

    public void JoinRoomButton()
    {     
        ChooseTypePanel.SetActive(false); 
        LobbyPanel.SetActive(true);
    }
    public void CreateRoomButton()
    {     
        ChooseTypePanel.SetActive(false); 
        CreateRoomPanel.SetActive(true);
    }

    public void ChooseGameModeButton()
    {     
        CreateRoomPanel.SetActive(false); 
        RoomValuesPanel.SetActive(true);
    }

    public void BackMultiplayerButton()
    {     
        OnlineOfflinePanel.SetActive(true); 
        ConnectPanel.SetActive(false);
    }

    public void BackConnectButton()
    {     
        ConnectPanel.SetActive(true); 
        ChooseTypePanel.SetActive(false);
    }

    public void BackChooseTypeButton()
    {     
        ChooseTypePanel.SetActive(true); 
        CreateRoomPanel.SetActive(false);
    }

    public void BackJoinRoom()
    {     
        ChooseTypePanel.SetActive(true); 
        LobbyPanel.SetActive(false);
    }

    public void BackCreateRoom()
    {     
        CreateRoomPanel.SetActive(true); 
        RoomValuesPanel.SetActive(false);
    }

    public void BackRoom()
    {
        ChooseTypePanel.SetActive(true); 
        FinalRoomPanel.SetActive(false);
        FinalRoomPanel2.SetActive(false);
    }

    public void FinalRoom(int i)
    {
        if (i == 0)
            FinalRoomPanel.SetActive(true);
        else
            FinalRoomPanel2.SetActive(true);
        CreateRoomPanel.SetActive(false);
        RoomValuesPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    }
    */
}