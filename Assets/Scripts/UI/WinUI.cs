using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WinUI : MonoBehaviour
{
    /// <summary>
    /// The text to display the winner on
    /// </summary>
    public TextMeshProUGUI _text = null;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = "P" + GameManager.s_winningPlayer + " Well Played!";
    }
}
