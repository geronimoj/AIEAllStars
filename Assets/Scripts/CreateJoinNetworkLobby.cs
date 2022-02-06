using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateJoinNetworkLobby : MonoBehaviour
{
    /// <summary>
    /// Is the lobby private
    /// </summary>
    private bool _isPrivateLobby = false;
    /// <summary>
    /// The name of the room to make
    /// </summary>
    private string _roomName = string.Empty;
    /// <summary>
    /// Set the private state of the lobby
    /// </summary>
    /// <param name="privateness">Is the room private</param>
    public void SetPrivate(bool privateness) => _isPrivateLobby = privateness;
    /// <summary>
    /// Set the name of the room to create or join
    /// </summary>
    /// <param name="name">The name of the room to create or join</param>
    public void SetRoomName(string name) => _roomName = name;

    public void CreateRoom()
    {
        NetworkManager.s_instance.CreateRoom(_roomName, _isPrivateLobby);
    }

    public void JoinRoom()
    {
        NetworkManager.s_instance.JoinRoom(_roomName);
    }
}
