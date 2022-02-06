using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitCollider))]
public class FirePillar : MonoBehaviour
{
    HitCollider col;
    Collider fireCollider;

    public Player caster;

    private bool networkedOwnerInvert = false;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<HitCollider>();

        fireCollider = GetComponent<Collider>();

        if (col == null)
            Debug.LogError("Could not find Collider");

        if (caster == null)
            GetAttacker();

        col.SetAttacker(caster.transform);

        StartCoroutine(WaitBeforeExplode());

        Destroy(gameObject, 2);
    }

    IEnumerator WaitBeforeExplode()
    {
        yield return new WaitForSeconds(.5f);

        Explode();
    }

    void Explode()
    {
        fireCollider.enabled = true;

        StartCoroutine(WaitTillColDisable());
    }

    IEnumerator WaitTillColDisable()
    {
        yield return new WaitForSeconds(0.5f);

        fireCollider.enabled = false;
    }
    protected void GetAttacker()
    {
        if (NetworkManager.InRoom)
        {
            int index;

            if (networkedOwnerInvert)
                index = NetworkManager.AmHost ? 0 : 1;
            else
                index = NetworkManager.AmHost ? 1 : 0;

            caster = GameManager.s_instance._players[index];
        }
    }
}
