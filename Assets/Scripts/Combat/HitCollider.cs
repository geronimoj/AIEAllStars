using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitCollider : MonoBehaviour
{
    public float damage;

    public float enemyStunDuration;

    public float selfStunDuration;

    [Tooltip("0 is forwards relative to the character forwards, 90 is up")]
    public float launchAngle;

    public float launchForce;

    Transform attacker;
    public void SetAttacker(Transform me)
    {
        attacker = me;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if the other is a character, and if so hit them
        Player p = other.GetComponent<Player>();

        if (p)
        {
            if (p.transform == attacker)
                return;

            //Calculate laucnh force

            p.GotHit(damage, enemyStunDuration, Vector3.zero);
        }
    }
}
