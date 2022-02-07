using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(SceneLoader))]
[RequireComponent(typeof(DisconnectALoad))]
public class RematchSystem : MonoBehaviourPun
{
    /// <summary>
    /// Storage for each players want to rematch
    /// </summary>
    private bool[] _rematch = new bool[2];

    private void Update()
    {   //If we are not networked, don't need this running
        if (!PhotonNetwork.IsConnected)
        {
            Destroy(this);
            return;
        }
        //If the other player leaves, leave ourself
        if (NetworkManager.AmAlone)
            GetComponent<DisconnectALoad>().Disconnect();
    }

    public void Rematch(bool wantRematch)
    {
        int index = NetworkManager.AmHost ? 0 : 1;
        _rematch[index] = wantRematch;
        //Tell other player about our want to rematch
        photonView.RPC("SyncRematch", RpcTarget.Others, wantRematch);
        //Check the rematch conditions
        DoRematch();
    }

    [PunRPC]
    private void SyncRematch(bool wantRematch)
    {
        int index = NetworkManager.AmHost ? 1 : 0;
        _rematch[index] = wantRematch;
        //Check the rematch conditions
        DoRematch();
    }
    /// <summary>
    /// Checks if a rematch should occur, if so, starts a rematch
    /// </summary>
    private void DoRematch()
    {   //If we are the host and both players agree to a rematch, load the lobby scene
        if (NetworkManager.AmHost && _rematch[0] == true && _rematch[0] == _rematch[1])
        {   //Load the lobby
            SceneLoader sl = GetComponent<SceneLoader>();
            sl.LoadScene("Lobby");
        }
    }
}
