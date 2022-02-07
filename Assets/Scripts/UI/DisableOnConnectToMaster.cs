using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnConnectToMaster : MonoBehaviour
{
    [SerializeField]
    [Tooltip("To enable or disable the object when connecting")]
    private bool enable = false;

    [SerializeField]
    [Tooltip("Object to enable or disable upon connecting")]
    private GameObject target = null;

    [SerializeField]
    private float delay = 0;

    void Update()
    {   //Disable the gameObject when we connect to the server
        if (Photon.Pun.PhotonNetwork.IsConnectedAndReady)
            StartCoroutine(SetActive());
    }

    private IEnumerator SetActive()
    {
        if (delay <= 0)
        {
            if (target)
                target.SetActive(enable);
            else
                gameObject.SetActive(enable);
            this.enabled = false;
            yield break;
        }

        yield return new WaitForSeconds(delay);
        if (target)
            target.SetActive(enable);
        else
            gameObject.SetActive(enable);
        this.enabled = false;
    }
}
