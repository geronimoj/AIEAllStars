using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    public bool IsWon
    {
        set
        {
            front.SetActive(value);
        }
    }

    public GameObject front = null;
}
