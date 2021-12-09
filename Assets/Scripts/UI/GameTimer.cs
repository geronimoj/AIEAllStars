using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI _timeText = null;

    private void Awake()
    {
        _timeText = GetComponent<TextMeshProUGUI>();
        _timeText.text = "99";
    }

    private void LateUpdate()
    {   //Make sure there is an instance
        if (!GameManager.s_instance || GameManager.s_instance.m_startTime == 0)
            return;

        GameManager m = GameManager.s_instance;
        _timeText.text = ((int)(m.m_startTime + m._maxGameTime - Time.time)).ToString();
    }
}
