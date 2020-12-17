using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ConectionManager : MonoBehaviourPunCallbacks, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks
{
    #region VARIABLES
    public LogWriter logWriter;

    // Test variables
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
        
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers, CustomRoomPropertiesForLobby = GetCustomRoomPropertiesForLobby(), CustomRoomProperties = customRoomProperties });
    }

    // QUE PASA SI ESTA LLENA ????????????????????????
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

    /// <summary>
    /// TESTEO
    /// Crea una sala con el nombre del nickname del cliente + "Room" y 10 jugadores máximo
    /// </summary>
    public void CreateRoom()
    {
        string roomName = PhotonNetwork.NickName + "Room";
        byte maxPlayers = 10;

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();   // Valor por defecto de sala (modo caza)
        customRoomProperties["gameMode"] = GameMode.Hunt;

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

    #region PUN CALLBACKS
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        logWriter.Write("Conectado al servidor" + PhotonNetwork.ServerAddress);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        logWriter.Write("Se ha desconectado por la causa: " + cause.ToString());
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        logWriter.Write("Se ha unido al lobby");
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        logWriter.Write("Se ha marchado del lobby");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        logWriter.Write("--ROOMS UPDATE START--");
        foreach(RoomInfo roomInfo in roomList)
        {
            logWriter.Write(roomInfo.ToString() + ", GameMode: " + roomInfo.CustomProperties["gameMode"].ToString());
        }
        logWriter.Write("--ROOMS UPDATE END--");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        logWriter.Write("Sala creada");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        logWriter.Write("Unido a la sala " + PhotonNetwork.CurrentRoom.Name + " - nº players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        logWriter.Write("Fallo al unirse a sala: " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        logWriter.Write("Error al unirse a sala aleatoria: " + message);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        logWriter.Write("Creación de sala fallida por error: " + message);
    }
    #endregion

}
