using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmHost : MonoBehaviour
{
    public GameObject target = null;

    public bool enable = false;

    void Start()
    {
        if (NetworkManager.AmHost)
            target.SetActive(enable);
        else
            target.SetActive(!enable);
    }
}
