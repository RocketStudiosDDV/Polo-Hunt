using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntry : MonoBehaviour
{
    public string roomName;
    private ConectionManager conectionManager;

    private void Awake()
    {
        roomName = "";
        conectionManager = FindObjectOfType<ConectionManager>();
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public string GetRoomName()
    {
        return roomName;
    }

    public void EnterRoom()
    {
        conectionManager.JoinRoom(roomName);
    }
}
