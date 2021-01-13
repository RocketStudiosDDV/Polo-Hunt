using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Networking;

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
    public GameObject FinalRoomPanel;
    public GameObject FinalRoomPanel2;
    public Transform RoomPrefab;
    public Transform RoomPrefabContainer;
    public List<Transform> RoomPrefabList;

    public Transform ListRoomPrefab;
    public Transform ListRoomContainer;
    public List<Transform> ListRoomPrefabList;

    public GameObject errorJoinTxt;

    public Text room1Txt;
    public Text room2Txt;

    public string RoomName;

    public string name1;
    public string psw1;

    private int createRoomAttempt;

    private int language;

    public class Player1
    {
        public string name;
        public string psw;
    }

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
            if(logWriter != null)
                logWriter.Write("Ya está conectado", true);
        } else
        {
            if (logWriter != null)
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
        if(logWriter != null)
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
        name1 = nickName;
    }

    

    /// <summary>
    /// Settea la password del cliente
    /// </summary>
    /// <param name="psw"></param>
    public void SetPassword(string psw)
    {
        psw1 = psw;
    }

    /// <summary>
    /// Une al cliente al lobby (único)
    /// Callback de OnJoinedLobby
    /// </summary>
    public void JoinLobby()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (logWriter != null)
                logWriter.Write("Debe estar conectado al servidor primero");
        } else if (PhotonNetwork.InRoom)
        {
            if (logWriter != null)
                logWriter.Write("Debe estar fuera de una sala primero");
        } else
        {
            if (PhotonNetwork.InLobby)
            {
                if (logWriter != null)
                    logWriter.Write("Ya está en un lobby");
            }
            else
            {
                if (logWriter != null)
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
            if (logWriter != null)
                logWriter.Write("Dejando lobby...");
            PhotonNetwork.LeaveLobby();
        } else
        {
            if (logWriter != null)
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
        if (logWriter != null)
            logWriter.Write("Creando sala...");
        
        if (roomName == null || roomName == "")
        {
            roomName = PhotonNetwork.NickName + "Room";
        }

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();   // Valor por defecto de sala (modo caza)
        customRoomProperties["gameMode"] = GameMode.Hunt;
        customRoomProperties["hasStarted"] = false;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties, BroadcastPropsChangeToAll = true });
    }

    public void CreateRoom2(byte maxPlayers = 10)
    {
        if (logWriter != null)
            logWriter.Write("Creando sala...");
        
        if (RoomName == null || RoomName == "")
        {
            RoomName = PhotonNetwork.NickName + "Room";
        }

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();   // Valor por defecto de sala (modo caza)
        customRoomProperties["gameMode"] = GameMode.Hunt;
        customRoomProperties["hasStarted"] = false;

        PhotonNetwork.CreateRoom(RoomName, new RoomOptions { MaxPlayers = maxPlayers, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties, BroadcastPropsChangeToAll = true });
    }

    /// <summary>
    /// Crea una sala privada con el nombre y el nº máximo de jugadores pasados como parámetro.
    /// Para unirse a la sala sólo hace falta el nombre.
    /// Nombre default = nickname del cliente + "Room"
    /// maxPlayers default = 10
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="maxPlayers"></param>
    public void CreatePrivateRoom()
    {
        if (logWriter != null)
            logWriter.Write("Creando sala privada...");

        if (RoomName == null || RoomName == "")
        {
            RoomName = PhotonNetwork.NickName + "Room";
        }

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();   // Valor por defecto de sala (modo caza)
        customRoomProperties["gameMode"] = GameMode.Hunt;
        customRoomProperties["hasStarted"] = false;

        PhotonNetwork.CreateRoom(RoomName, new RoomOptions { MaxPlayers = 10, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties, IsVisible = false });
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
        if (logWriter != null)
            logWriter.Write("Intentando unirse a la sala " + roomName + "...");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRoom2()
    {
        if (logWriter != null)
            logWriter.Write("Intentando unirse a la sala " + RoomName + "...");
        PhotonNetwork.JoinRoom(RoomName);
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
        PlayerListZero();
    }

    /// <summary>
    /// Imprime el nombre del lobby actual
    /// </summary>
    public void PrintLobbyName()
    {
        if (PhotonNetwork.InLobby)
        {
            if (logWriter != null)
                logWriter.Write(PhotonNetwork.CurrentLobby);
        } else
        {
            if (logWriter != null)
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
            else
            {
                SetNumberOfBears(1);
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
    /// Settea la propiedad hasStarted a true
    /// Sólo lo puede llamar el MasterClient (host)
    /// </summary>
    public void StartMatch()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    ExitGames.Client.Photon.Hashtable newCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                    newCustomRoomProperties["hasStarted"] = true;
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomRoomProperties);
                    object customProperty;
                    int modeSelected = 0;
                    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out customProperty))
                        modeSelected = (int)customProperty;
                    switch (modeSelected)
                    {
                        case 0:
                            PhotonNetwork.LoadLevel("HuntMap");
                            break;
                        case 1:
                            PhotonNetwork.LoadLevel("RaceMap");
                            break;
                    }
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
        int i = 0;
        int high = 100;
        foreach(Player player in players.Values)
        {
            playersList.Add(player.NickName);
            Transform entryTransform = Instantiate(RoomPrefab, RoomPrefabContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector3(0,-high * i, 0);
            entryTransform.gameObject.SetActive(true);
            entryTransform.Find("Text").GetComponent<Text>().text = player.NickName;
            RoomPrefabList.Add(entryTransform);
            i++;
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
        string[] customRoomPropertiesForLobby = { "gameMode", "hasStarted" };
        return customRoomPropertiesForLobby;
    }
    private void HideErrorJoin()
    {
        if (errorJoinTxt != null)
            errorJoinTxt.SetActive(false);
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
        customRoomProperties["hasStarted"] = false;

        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties });
    }

    /// <summary>
    /// TESTEO
    /// Se une a la sala con el nombre en la variable testRoomName
    /// </summary>
    public void JoinRoom()
    {
        if (logWriter != null)
            logWriter.Write("Intentando unirse a la sala " + testRoomName + "...");
        PhotonNetwork.JoinRoom(testRoomName);
    }
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        language = PlayerPrefs.GetInt("language", 0);
        createRoomAttempt = 0;

        //RoomPrefabContainer = transform.Find("RoomPrefabContainer");
        //RoomPrefab = RoomPrefabContainer.Find("RoomPrefab");

        RoomPrefab.gameObject.SetActive(false);
        ListRoomPrefab.gameObject.SetActive(false);

        if (PhotonNetwork.InRoom)
        {
            LobbyPanel.SetActive(false);
            ConnectPanel.SetActive(false);
            RoomPanel.SetActive(false);
            ChooseTypePanel.SetActive(false);
            OnlineOfflinePanel.SetActive(false);
            CreateRoomPanel.SetActive(false);
            RoomValuesPanel.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
                FinalRoom(0);
            else
                FinalRoom(1);
        }
    }
    #endregion

    #region PUN CALLBACKS
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.ContainsKey("gameMode"))
        {
            if (logWriter != null)
                logWriter.Write("Modo de juego cambiado");
            // EL MODO DE JUEGO HA SIDO SETEADO
        } else if (propertiesThatChanged.ContainsKey("numberOfBears"))
        {
            if (logWriter != null) {
                object numBearsChanged;
                propertiesThatChanged.TryGetValue("numberOfBears", out numBearsChanged);
                logWriter.Write("Nº osos cambiado: " + numBearsChanged.ToString());
            }

            // EL Nº DE OSOS HA SIDO SETEADO
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (logWriter != null)
            logWriter.Write("Conectado al servidor" + PhotonNetwork.ServerAddress);
        if (PhotonNetwork.LocalPlayer.NickName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = "player";
        }
        ConnectButton();
    }

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
        PlayerListZero();
        GetPlayersList();
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
        PlayerListZero();
        GetPlayersList();
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
    }

    public override void OnLeftLobby()
    {        
        base.OnLeftLobby();
        RoomListZero();
        if (logWriter != null)
            logWriter.Write("Se ha marchado del lobby");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomListZero();
        base.OnRoomListUpdate(roomList);
        int i=0;
        int high = 100;
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo != null && roomInfo.PlayerCount > 0)
            {
                Transform entryTransform = Instantiate(ListRoomPrefab, ListRoomContainer);
                RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
                entryRectTransform.anchoredPosition = new Vector3(0, -high * i, 0);
                entryTransform.gameObject.SetActive(true);
                entryTransform.GetComponent<RoomEntry>().SetRoomName(roomInfo.Name);
                Debug.Log(roomInfo.Name);
                entryTransform.Find("RoomText").GetComponent<Text>().text = roomInfo.Name;
                entryTransform.Find("PlayersText").GetComponent<Text>().text = roomInfo.PlayerCount + "/10";
                try
                {
                    if (roomInfo.CustomProperties["gameMode"].ToString().CompareTo("0") == 1)
                        entryTransform.Find("ModeText").GetComponent<Text>().text = "RACE";
                    else
                        entryTransform.Find("ModeText").GetComponent<Text>().text = "POLO-HUNT";
                    if ((bool)roomInfo.CustomProperties["hasStarted"] == true)
                    {
                        if (language == 0)                           
                            entryTransform.Find("AvaliableText").GetComponent<Text>().text = "playing";
                        else
                            entryTransform.Find("AvaliableText").GetComponent<Text>().text = "en partida";
                    }
                    else
                    {
                        if (language == 0)
                            entryTransform.Find("AvaliableText").GetComponent<Text>().text = "avaliable";
                        else
                            entryTransform.Find("AvaliableText").GetComponent<Text>().text = "disponible";
                    }
                }
                catch (System.Exception ex)
                {

                }
                ListRoomPrefabList.Add(entryTransform);
                i++;
                /*
                if (roomInfo != null)
                    logWriter.Write(roomInfo.ToString() + ", GameMode: " + roomInfo.CustomProperties["gameMode"].ToString() + ", Started: " + roomInfo.CustomProperties["hasStarted"].ToString());
                */
            }
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
        errorJoinTxt.SetActive(true);
        Invoke(nameof(HideErrorJoin), 4);
        //ChooseTypePanel.SetActive(true);
        //LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        ConnectPanel.SetActive(false);
        FinalRoomPanel.SetActive(false);
        FinalRoomPanel2.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        if (logWriter != null)
            logWriter.Write("Error al unirse a sala aleatoria: " + message);

        ChooseTypePanel.SetActive(true);
        HideErrorJoin();
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        ConnectPanel.SetActive(false);
        FinalRoomPanel.SetActive(false);
        FinalRoomPanel2.SetActive(false);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        if (logWriter != null)
            logWriter.Write("Creación de sala fallida por error: " + message);
        if (returnCode == ErrorCode.GameIdAlreadyExists)
        {
            createRoomAttempt++;
            CreateRoom(PhotonNetwork.LocalPlayer.NickName + "Room" + createRoomAttempt);
        } else
        {
            ChooseTypePanel.SetActive(true);
            HideErrorJoin();
            LobbyPanel.SetActive(false);
            RoomPanel.SetActive(false);
            ConnectPanel.SetActive(false);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (logWriter != null)
            logWriter.Write("Sala abandonada");

        ChooseTypePanel.SetActive(true);
        HideErrorJoin();
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        ConnectPanel.SetActive(false);
        FinalRoomPanel.SetActive(false);
        FinalRoomPanel2.SetActive(false);
        PlayerListZero();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        LeaveRoom();
        PlayerListZero();
    }
    #endregion

    public void Insc()
    {
       StartCoroutine(InscribeUser());
    }

    public void Rec()
    {
       StartCoroutine(SeeUser());
    }

    public IEnumerator InscribeUser()
    {
        Player1 p = new Player1();
        p.name = name1;
        p.psw = psw1;

        string data = JsonUtility.ToJson(p);

        UnityWebRequest www = UnityWebRequest.Post("http://polo-hunt.ddns.net:8800/", data);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
        
        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("www.error");
        }
        else if(www.downloadHandler.text == "f")
        {
            Debug.Log("Guardado nombre de usuario"+www.downloadHandler.text.ToString());
        }
        else
        {
            Debug.Log("Ese usuario ya existe"+www.downloadHandler.text.ToString());
        }
        
    }

    public IEnumerator SeeUser()
    {
        Player1 p = new Player1();
        p.name = name1;
        p.psw = psw1;

        string data = JsonUtility.ToJson(p);

        UnityWebRequest www = UnityWebRequest.Post("http://polo-hunt.ddns.net:8800/login/",data);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("www.error");
        }
        else
        {
            Debug.Log("Nombre de usuario correcto");
            Connect();
        }
    }

    public void SetRoomName(string name)
    {
        RoomName = name;
    }

    public void MultiplayerButton()
    {     
        OnlineOfflinePanel.SetActive(false); 
        ConnectPanel.SetActive(true);
    }

    public void ConnectButton()
    {     
        ConnectPanel.SetActive(false);
        ChooseTypePanel.SetActive(true);
        RoomListZero();
    }

    public void JoinRoomButton()
    {   
        ChooseTypePanel.SetActive(false);
        LobbyPanel.SetActive(true);
        JoinLobby();
    }
    public void CreateRoomButton()
    {     
        ChooseTypePanel.SetActive(false);
        LeaveLobby();
        CreateRoomPanel.SetActive(true);
    }

    public void ChooseGameModeButton()
    {     
        FinalRoomPanel.SetActive(false);
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
        LeaveLobby();
        RoomListZero();
        HideErrorJoin();
        LobbyPanel.SetActive(false);
    }

    public void BackCreateRoom()
    {
        FinalRoomPanel.SetActive(true);
        RoomValuesPanel.SetActive(false);
    }

    public void BackRoom()
    {
        ChooseTypePanel.SetActive(true); 
        FinalRoomPanel.SetActive(false);
        FinalRoomPanel2.SetActive(false);
        PlayerListZero();
    }

    public void PlayerListZero()
    {
        foreach(Transform t in RoomPrefabList)
        {
            t.gameObject.SetActive(false);
            Destroy(t.gameObject);
        }
        RoomPrefabList.Clear() ;
    }

    public void RoomListZero()
    {
        foreach(Transform t in ListRoomPrefabList)
        {
            t.gameObject.SetActive(false);
            Destroy(t.gameObject);
        }
        ListRoomPrefabList.Clear() ;
    }

    public void FinalRoom(int i)
    {
        GetPlayersList();
        if (i == 0)
        {
            FinalRoomPanel.SetActive(true);
            room1Txt.text = GetRoomName();
        }
        else
        {
            FinalRoomPanel2.SetActive(true);
            room2Txt.text = GetRoomName();
        }
        CreateRoomPanel.SetActive(false);
        RoomValuesPanel.SetActive(false);
        HideErrorJoin();
        LobbyPanel.SetActive(false);
        RoomListZero();
        LeaveLobby();
    }
}