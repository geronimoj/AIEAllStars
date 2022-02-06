using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomUI : MonoBehaviour
{
    private RoomInfo m_room = null;

    public RoomInfo Room
    {
        set
        {
            m_room = value;

            if (m_room == null)
            {
                gameObject.SetActive(false);
                return;
            }
            //Set room name
            if (_roomNameText != null)
                _roomNameText.text = m_room.Name;
        }
    }

    [SerializeField]
    private TextMeshProUGUI _roomNameText = null;

    public void Join()
    {   //Join the room
        NetworkManager.s_instance.JoinRoom(m_room.Name);
    }
}
