using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public Vector3 spawnOffset;

    public void Fire(Object projectile)
    {
        GameObject proj = projectile as GameObject;

        if (!proj)
        {
            Debug.LogError("Event gave invalid parameter");
            return;
        }

        Vector3 offset = spawnOffset.x * transform.right + spawnOffset.y * transform.up + spawnOffset.z * transform.forward;

        proj = Instantiate(proj, transform.position + offset, Quaternion.LookRotation(transform.forward, Vector3.up));
        proj.GetComponent<Bullet>().SetAttacker(transform);
    }
}
