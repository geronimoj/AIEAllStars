using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomBrowser : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _uiParent = null;

    [SerializeField]
    private RoomUI _uiPrefab = null;

    private List<RoomUI> _spawnedUI = new List<RoomUI>();
    /// <summary>
    /// Anti garbage collection measures.
    /// </summary>
    private List<RoomInfo> _temp = null;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList == null)
            return;
        //Store a reference so it doesn't get cleaned up by garbage collection
        _temp = roomList;
        //Get the list with the longest length
        int length = roomList.Count > _spawnedUI.Count ? roomList.Count : _spawnedUI.Count;

        for (int i = 0; i < length; i++)
        {   //If there are no more rooms to display, disable UI prompts
            if (i >= roomList.Count)
            {   //Set the target to null, it will disable itself
                _spawnedUI[i].Room = null;
                continue;
            }
            //Create a new UI object
            if (i >= _spawnedUI.Count)
            {
                GameObject obj = Instantiate(_uiPrefab.gameObject, _uiParent);
                //Store the UI object
                _spawnedUI.Add(obj.GetComponent<RoomUI>());
            }
            //Store the room
            _spawnedUI[i].Room = roomList[i];
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
}
