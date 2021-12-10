using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public void Fire(Object projectile)
    {
        GameObject proj = projectile as GameObject;

        if (!proj)
        {
            Debug.LogError("Event gave invalid parameter");
            return;
        }

        proj = Instantiate(proj, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
        proj.GetComponent<Bullet>().SetAttacker(transform);
    }
}
