using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ConectionManager : MonoBehaviourPunCallbacks, IConnectionCallbacks, ILobbyCallbacks
{
    #region VARIABLES
    public LogWriter logWriter;
    public string testRoomString;
    #endregion

    #region UNITY CALLBACKS
    #endregion

    #region METHODS
    /// <summary>
    /// Conecta el cliente al servidor de Photon
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

    public void Disconnect()
    {
        logWriter.Write("Desconectándose...");
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// Une al cliente al lobby de Polo Hunt
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
    /// Settea el nickname del cliente
    /// </summary>
    /// <param name="nickName"></param>
    public void SetNickName(string nickName)
    {
        PhotonNetwork.NickName = nickName;
    }

    /// <summary>
    /// Crea una sala con el nombre pasado como parámetro
    /// Si no pasa nombre la crea con el nickname del usuario
    /// </summary>
    /// <param name="roomName"></param>
    public void CreateRoom(string roomName = "", byte maxPlayers = 10)
    {
        logWriter.Write("Creando sala...");
        if (roomName == null || roomName == "")
        {
            roomName = PhotonNetwork.NickName + "Room";
        }
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers });
    }

    public void CreateRoom()
    {
        string roomName = PhotonNetwork.NickName + "Room";
        byte maxPlayers = 10;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers });
    }

    // QUE PASA SI ESTA LLENA ????????????????????????
    // HACERLA CON PARAMETRO DE NOMBRE DE SALA!!!!!!!!!!!!
    public void JoinRoom()
    {
        logWriter.Write("Intentando unirse a la sala " + testRoomString + "...");
        PhotonNetwork.JoinRoom(testRoomString);
    }

    public void TestSetRoomName(string roomName)
    {
        testRoomString = roomName;
    }
    #endregion

    #region PUN CALLBACKS
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        logWriter.Write("Conectado al servidor");
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
            logWriter.Write(roomInfo.ToString());
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
        logWriter.Write("Unido a la sala " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        logWriter.Write("Creación de sala fallida por error: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        logWriter.Write("Error al unirse a la sala: " + message);
    }
    #endregion

}
