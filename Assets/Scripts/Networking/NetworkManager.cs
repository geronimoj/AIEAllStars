using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
//Wish it didn't have to be a behaviour
public class NetworkManager : MonoBehaviour
{
    #region Static
    public static NetworkManager s_instance = null;
    /// <summary>
    /// Returns true if we are the master client and connected
    /// </summary>
    public static bool AmHost => (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient) || !PhotonNetwork.IsConnected;
    /// <summary>
    /// Is true while the player is in a room/lobby
    /// </summary>
    public static bool InRoom => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom;
    /// <summary>
    /// Determines if a given photon view is owned by this player
    /// </summary>
    /// <param name="obj">The photonView to check</param>
    /// <returns>Returns false if you are not connected to photon servers or it is not yours</returns>
    public static bool IsMine(PhotonView obj)
    {   //Check if it is ours and that we are connected to photon servers
        return PhotonNetwork.IsConnectedAndReady && obj.IsMine;
    }
    /// <summary>
    /// Determines if a given photon view is owned by this player
    /// </summary>
    /// <param name="obj">The photonView to check</param>
    /// <returns>Returns false if you are not connected to photon servers or it is not yours. Also returns true if the object is not a networked object</returns>
    public static bool IsMine(GameObject obj)
    {   //Attempt to get a photon view from the object
        PhotonView view = obj.GetComponent<PhotonView>();
        //If there is none, we presume its not a networked object and such is ours
        if (!view)
            return true;
        else
            return IsMine(view);
    }
    /// <summary>
    /// Checks if the player is currently alone in the room. If the player is not connected to the servers, returns true
    /// </summary>
    public static bool AmAlone => !PhotonNetwork.IsConnected || (InRoom && PhotonNetwork.PlayerList.Length < 2);

    public static void ConnectToServers()
    {   //Connect to the photon servers
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public static void DisconnectFromServers()
    {   //Disconnect from the photon servers
        if (!PhotonNetwork.IsConnected)
            return;

        if (PhotonNetwork.InRoom)
            DisconnectFromRoom();

        PhotonNetwork.Disconnect();
        //Destroy the networkManager when we disconnect
        Destroy(s_instance.gameObject);
        s_instance = null;
    }

    public static void DisconnectFromRoom()
    {   //Exit the room
        if (InRoom)
            PhotonNetwork.LeaveRoom();
    }
    #endregion

    private void Awake()
    {   //Store reference to instance
        s_instance = this;
        //Connect to photon
        ConnectToServers();
        //Don't destroy this. We need it for other network based activities
        DontDestroyOnLoad(this);
        //Make sure scenes are synced.
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnDestroy()
    {   //Make sure the instance has been propperly nulled
        s_instance = null;
    }

    public void CreateRoom(string roomName, bool isPrivate)
    {   //Create a room with a custom property for isPrivate
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties.Add(0, isPrivate);
        //Also make sure the room is not visible if its private
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { IsVisible = !isPrivate, MaxPlayers = 2, CustomRoomProperties = roomProperties });

        Debug.Log("Creating " + (isPrivate ? "private" : "open") + " room");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void QuickPlay()
    {   //Search for a room that is not marked as private
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties.Add(0, false);
        PhotonNetwork.JoinRandomRoom(roomProperties, 2);

        Debug.Log("Searching for public room");
    }
}
