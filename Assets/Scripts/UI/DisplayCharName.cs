using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayCharName : MonoBehaviour
{
    private TextMeshProUGUI text;

    public byte player = 0;

    private void Start()
    {   //Null catch both because why not
        if (!CharacterSelector.s_p2Selected || !CharacterSelector.s_p1Selected)
            return;

        text = GetComponent<TextMeshProUGUI>();

        text.text = player == 0 ? CharacterSelector.s_p1Selected.Name : CharacterSelector.s_p2Selected.Name;
    }
}
