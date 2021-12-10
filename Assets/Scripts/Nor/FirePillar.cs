using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitCollider))]
public class FirePillar : MonoBehaviour
{
    HitCollider col;
    Collider fireCollider;

    public Player caster;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<HitCollider>();

        fireCollider = GetComponent<Collider>();

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
}
