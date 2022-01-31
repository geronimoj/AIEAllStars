using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager
{
    #region Static
    /// <summary>
    /// Returns true if we are the master client and connected
    /// </summary>
    public static bool AmHost => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient;
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

    public static void ConnectToServers()
    {   //Connect to the photon servers
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public static void DisconnectFromServers()
    {   //Disconnect from the photon servers
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    public static void DisconnectFromRoom()
    {   //Exit the room
        if (InRoom)
            PhotonNetwork.LeaveRoom();
    }
    #endregion
}
