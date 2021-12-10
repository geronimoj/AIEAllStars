using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggler : MonoBehaviour
{
    public GameObject target;

    public void ToggleOn()
    {
        target.SetActive(true);
    }

    public void ToggleOff()
    {
        target.SetActive(false);
    }
}
