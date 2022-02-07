using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(SceneLoader))]
public class DisconnectALoad : MonoBehaviourPunCallbacks
{
    public void Disconnect()
    {
        //If in a networked setting, return to multiplayer or main menu
        SceneLoader sc = GetComponent<SceneLoader>();
        if (NetworkManager.InRoom)
        {   //Exit the room
            NetworkManager.DisconnectFromRoom();
        }
        else if (Photon.Pun.PhotonNetwork.IsConnected)
        {   //Disconnect from photon
            NetworkManager.DisconnectFromServers();
        }   //Not networked so just load the scene
        else
            sc.LoadScene("MainMenu");
    }

    public override void OnLeftRoom()
    {
        SceneLoader sc = GetComponent<SceneLoader>();
        sc.LoadScene("Multiplayer");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneLoader sc = GetComponent<SceneLoader>();
        sc.LoadScene("MainMenu");
    }
}