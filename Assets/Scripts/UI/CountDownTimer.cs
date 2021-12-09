using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDownTimer : MonoBehaviour
{
    /// <summary>
    /// If the timer is 0, display this text instead
    /// </summary>
    public string m_timerZero = "GO!";

    public TextMeshProUGUI _text = null;

    private float _timer = 0;

    private void Start()
    {
        _timer = GameManager.s_instance._startCountDown + 1;
    }

    private void LateUpdate()
    {   //Store instance
        _timer -= Time.deltaTime;

        int value = (int)_timer;

        if (value < 0 || _timer <= 0)
        {   //Disable, destroy and return
            gameObject.SetActive(false);
            Destroy(this);
            return;
        }
        //Display text
        _text.text = value == 0 ? m_timerZero : value.ToString();
    }
}
