using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class GameConnectDebug : MonoBehaviourPunCallbacks
{
    public UnityEvent OnJoinRoom;

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined an existing room with " + (NetworkManager.AmAlone ? "Nobody" : "Somebody"));
        OnJoinRoom.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {   //If fail, create room instead
        NetworkManager.s_instance.CreateRoom(null, false);
    }
}
