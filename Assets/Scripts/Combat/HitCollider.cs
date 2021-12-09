﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitCollider : MonoBehaviour
{
    [Tooltip("damage this attack will do")]
    public float damage;

    [Tooltip("how long it will stun the person hit for")]
    public float enemyStunDuration;

    [Tooltip("if another input isn't pressed, you will be unable to attack again for this amount of time")]
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
            Vector3 angle = transform.forward * Mathf.Sin(launchAngle) + transform.up * Mathf.Cos(launchAngle);
            angle.Normalize();

            p.GotHit(damage, enemyStunDuration, angle * launchForce);
        }
    }
}
