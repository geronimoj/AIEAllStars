using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectFromPhoton : MonoBehaviour
{
    public void Disconnect()
    {   //Disconnect from photon
        NetworkManager.DisconnectFromServers();
    }
}
