using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ConectionManager : MonoBehaviourPunCallbacks, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks
{
    #region VARIABLES
    // PUBLICAS
    public LogWriter logWriter;

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

    // TEST
    public string testRoomName;
    public bool testRoomProperty;
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Conecta el cliente al servidor de Photon (settings en el inspector)
    /// Callbacks OnConnectedToMaster y OnDisconnected
    /// </summary>
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            logWriter.Write("Ya está conectado", true);
        } else
        {
            logWriter.Write("Conectándose...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// Desconecta el cliente del servidor de Photon
    /// Callback OnDisconnected
    /// </summary>
    public void Disconnect()
    {
        logWriter.Write("Desconectándose...");
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// Settea el nickname del cliente
    /// </summary>
    /// <param name="nickName"></param>
    public void SetNickName(string nickName)
    {
        PhotonNetwork.NickName = nickName;
    }

    /// <summary>
    /// Une al cliente al lobby (único)
    /// Callback de OnJoinedLobby
    /// </summary>
    public void JoinLobby()
    {
        if (!PhotonNetwork.IsConnected)
        {
            logWriter.Write("Debe estar conectado al servidor primero");
        } else if (PhotonNetwork.InRoom)
        {
            logWriter.Write("Debe estar fuera de una sala primero");
        } else
        {
            if (PhotonNetwork.InLobby)
            {
                logWriter.Write("Ya está en un lobby");
            }
            else
            {
                logWriter.Write("Conectándose al lobby...");
                PhotonNetwork.JoinLobby();
            }
        }      
    }

    /// <summary>
    /// Abandona el lobby actual
    /// Callback OnLeftLobby
    /// </summary>
    public void LeaveLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            logWriter.Write("Dejando lobby...");
            PhotonNetwork.LeaveLobby();
        } else
        {
            logWriter.Write("No está en ningún lobby");
        }
    }

    /// <summary>
    /// Crea una sala con el nombre y el nº maximo de jugadores pasados como parámetro
    /// Nombre default = nickname del cliente + "Room"
    /// maxPlayers default = 10
    /// </summary>
    /// <param name="roomName"></param>
    public void CreateRoom(string roomName = "", byte maxPlayers = 10)
    {
        logWriter.Write("Creando sala...");
        
        if (roomName == null || roomName == "")
        {
            roomName = PhotonNetwork.NickName + "Room";
        }

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();   // Valor por defecto de sala (modo caza)
        customRoomProperties["gameMode"] = GameMode.Hunt;
        
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties, BroadcastPropsChangeToAll = true });
    }

    /// <summary>
    /// Crea una sala privada con el nombre y el nº máximo de jugadores pasados como parámetro.
    /// Para unirse a la sala sólo hace falta el nombre.
    /// Nombre default = nickname del cliente + "Room"
    /// maxPlayers default = 10
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="maxPlayers"></param>
    public void CreatePrivateRoom(string roomName)
    {
        logWriter.Write("Creando sala privada...");

        if (roomName == null || roomName == "")
        {
            roomName = PhotonNetwork.NickName + "Room";
        }

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();   // Valor por defecto de sala (modo caza)
        customRoomProperties["gameMode"] = GameMode.Hunt;

        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 10, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties, IsVisible = false });
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

    /// <summary>
    /// Se une a la sala con el nombre pasado como parámetro
    /// Callbacks OnJoinedRoom y OnJoinRoomFailed
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinRoom(string roomName)
    {
        logWriter.Write("Intentando unirse a la sala " + roomName + "...");
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// Se une a una sala aleatoria
    /// Callbacks OnJoinedRoom y OnJoinRandomFailed
    /// </summary>
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Desconecta al usuario de la sala y del lobby.
    /// Llama al callback OnLeftRoom
    /// </summary>
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    /// <summary>
    /// Imprime el nombre del lobby actual
    /// </summary>
    public void PrintLobbyName()
    {
        if (PhotonNetwork.InLobby)
        {
            logWriter.Write(PhotonNetwork.CurrentLobby);
        } else
        {
            logWriter.Write("No está en ningún lobby");
        }
    }

    /// <summary>
    /// Settea el modo de juego (sólo si es el Master Client (host))
    /// Callback OnRoomPropertiesUpdate(), en el if de "modo de juego seteado"
    /// </summary>
    /// <param name="gameMode"></param>
    public void SetGameMode(GameMode gameMode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable newCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            newCustomRoomProperties["gameMode"] = gameMode;
            PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomRoomProperties);

            if (gameMode == GameMode.Race)
            {
                SetNumberOfBears(0);
            }
        }       
    }

    
    public void SetGameMode(int gameMode)
    {
        if (gameMode == 0)
            SetGameMode(GameMode.Race);
        else
            SetGameMode(GameMode.Hunt);
    }

    /// <summary>
    /// Asigna el número de osos de la partida
    /// Callback OnRoomPropertiesUpdate(), en el if que pone "numero de osos seteado"
    /// </summary>
    /// <param name="numberOfBears"></param>
    public void SetNumberOfBears(int numberOfBears)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable newCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            newCustomRoomProperties["numberOfBears"] = numberOfBears;
            PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomRoomProperties);
        }
    }

    /// <summary>
    /// Inicia la partida (cambia la escena al nivel de juego). 
    /// Sólo lo puede llamar el MasterClient (host)
    /// </summary>
    public void StartMatch()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                object customProperty;
                int modeSelected = 0;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out customProperty))
                    modeSelected = (int)customProperty;
                switch(modeSelected)
                {
                    case 0:
                        PhotonNetwork.LoadLevel("EscenarioPruebas");
                        break;
                    case 1:
                        PhotonNetwork.LoadLevel("RunnerMap");
                        break;
                }
            }
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
    #endregion

    #region PRIVATE METHODS
    /// <summary>
    /// Devuelve un array con las custom room properties disponibles en el lobby
    /// </summary>
    /// <returns></returns>
    private string[] GetCustomRoomPropertiesForLobby()
    {
        string[] customRoomPropertiesForLobby = { "gameMode" };
        return customRoomPropertiesForLobby;
    }
    #endregion

    #region TEST METHODS
    public void SetTestRoomProperty(bool value)
    {
        testRoomProperty = value;
    }
    
    public void SetTestRoomName(string roomName)
    {
        testRoomName = roomName;
    }
    public void SetCustomPropertiesTest()
    {
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
        {
            ["gameMode"] = testRoomProperty
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    public void SetNumberOfBearsTest()
    {
        SetNumberOfBears(2);
    }

    public void ChangeGameModeTest()
    {
        SetGameMode(GameMode.Race);
    }

    /// <summary>
    /// TESTEO
    /// Crea una sala con el nombre del nickname del cliente + "Room" y 10 jugadores máximo
    /// </summary>
    public void CreateRoom()
    {
        string roomName = PhotonNetwork.NickName + "Room";
        byte maxPlayers = 10;

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        customRoomProperties["gameMode"] = GameMode.Hunt;   // Valor por defecto de sala (modo caza)
        customRoomProperties["numberOfBears"] = 1;  // Valor por defecto de sala (1 oso)

        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties });
    }

    /// <summary>
    /// TESTEO
    /// Se une a la sala con el nombre en la variable testRoomName
    /// </summary>
    public void JoinRoom()
    {
        logWriter.Write("Intentando unirse a la sala " + testRoomName + "...");
        PhotonNetwork.JoinRoom(testRoomName);
    }
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    #region PUN CALLBACKS
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        FinalRoom(0);
        if (propertiesThatChanged.ContainsKey("gameMode"))
        {
            if (logWriter != null)
                logWriter.Write("Modo de juego cambiado");
            // EL MODO DE JUEGO HA SIDO SETEADO
        } else if (propertiesThatChanged.ContainsKey("numberOfBears"))
        {
            if (logWriter != null)
                logWriter.Write("Nº osos cambiado");
            // EL Nº DE OSOS HA SIDO SETEADO
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (logWriter != null)
            logWriter.Write("Conectado al servidor" + PhotonNetwork.ServerAddress);

        ConnectButton();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        if (logWriter != null)
            logWriter.Write("Se ha desconectado por la causa: " + cause.ToString());

        ConnectPanel.SetActive(true);
        ChooseTypePanel.SetActive(false); 
        LobbyPanel.SetActive(false);
        CreateRoomPanel.SetActive(false); 
        RoomValuesPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (logWriter != null)
            logWriter.Write("Se ha unido al lobby");

        ConnectButton();
    }

    public override void OnLeftLobby()
    {        
        base.OnLeftLobby();
        if (logWriter != null)
            logWriter.Write("Se ha marchado del lobby");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        if (logWriter != null)
        {
            logWriter.Write("--ROOMS UPDATE START--");
            foreach (RoomInfo roomInfo in roomList)
            {
                if (roomInfo != null)
                    logWriter.Write(roomInfo.ToString() + ", GameMode: " + roomInfo.CustomProperties["gameMode"].ToString());
            }
            logWriter.Write("--ROOMS UPDATE END--");
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        if (logWriter != null)
            logWriter.Write("Sala creada");

        FinalRoom(0);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (logWriter != null)
            logWriter.Write("Unido a la sala " + GetRoomName() + " - nº players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers);

        FinalRoom(PhotonNetwork.CurrentRoom.PlayerCount-1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        if (logWriter != null)
            logWriter.Write("Fallo al unirse a sala: " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        if (logWriter != null)
            logWriter.Write("Error al unirse a sala aleatoria: " + message);

        ChooseTypePanel.SetActive(true);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        ConnectPanel.SetActive(false);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        if (logWriter != null)
            logWriter.Write("Creación de sala fallida por error: " + message);

        ChooseTypePanel.SetActive(true);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        ConnectPanel.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (logWriter != null)
            logWriter.Write("Sala abandonada");

        ChooseTypePanel.SetActive(true);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        ConnectPanel.SetActive(false);
    }
    #endregion

    

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
}