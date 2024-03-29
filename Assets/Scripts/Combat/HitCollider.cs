﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitCollider : MonoBehaviour
{
    enum Follow
    {
        none,
        torso,
        weapon,
        collider,
    }

    [Tooltip("damage this attack will do")]
    public float damage;

    [Tooltip("how long it will stun the person hit for")]
    public float enemyStunDuration;

    [Tooltip("if another input isn't pressed, you will be unable to attack again for this amount of time")]
    public float selfStunDuration;

    [Tooltip("0 is forwards relative to the character forwards, 90 is up")]
    public float launchAngle;

    public float launchForce;

    [Tooltip("If true, will not use launch angle and will move away from attacker")]
    public bool autoLaunchAway = false;

    public bool followUser = true;

    public bool giveInvFrames = false;

    [Tooltip("Used by golden pheonix to tell the explosion on their special who the actual owner is. Its getting it mixed up")]
    public bool networkedOwnerInvert = false;

    public float lifeStealAmount = 0;

    [Header("FX")]
    public vfxObj particles;
    [SerializeField] Follow toFollow = Follow.none;
    vfxObj partInstance;

    protected Transform attacker;
    public Transform Attacker => attacker;
    public void SetAttacker(Transform me)
    {   //Null catch assignment
        if (me == null)
            return;

        attacker = me;
    }

    private void Awake()
    {   //Get the person who performed the attack in a networked room.
        //Basically, if we didn't spawn it, it must be the other persons
        GetAttacker();

        if (!particles)
            return;

        partInstance = Instantiate(particles, transform.position + particles.transform.position, particles.transform.rotation);

        switch (toFollow)
        {
            case Follow.none:
                break;
            case Follow.torso:
                partInstance.transform.SetParent(attacker);
                break;
            case Follow.weapon:
                CombatController cc = attacker.GetComponent<CombatController>();
                if (cc)
                {
                    partInstance.transform.SetParent(cc.weaponPoint);
                }
                break;
            case Follow.collider:
                partInstance.transform.SetParent(transform);
                break;
        }

        partInstance.Initialise();

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //Check if the other is a character, and if so hit them
        Player p = other.GetComponent<Player>();

        if (p)
        {
            if (p.transform == attacker)
                return;

            if (p.InvincibilityTime > 0)
                return;

            //Calculate laucnh force
            Vector3 angle;

            if (autoLaunchAway)
                angle = p.transform.position - attacker.position;
            else
                angle = transform.forward * Mathf.Sin(launchAngle) + transform.up * Mathf.Cos(launchAngle);
            angle.Normalize();

            p.GotHit(damage, enemyStunDuration, angle * launchForce);

            if (giveInvFrames)
            {
                //Make enemy invinsible for short time
                p.InvincibilityTime = 0.5f;
            }

            if (lifeStealAmount > 0)
            {
                Player us = attacker.GetComponent<Player>();
                if (us)
                {
                    us.Heal(lifeStealAmount);

                    CombatController cc = us.GetComponent<CombatController>();
                    cc.SetHandParticles(true);
                    cc.StartCoroutine(WaitBeforeHandDisable(cc));
                }
            }

            //Turn our collider off
            GetComponent<Collider>().enabled = false;
        }

    }

    IEnumerator WaitBeforeHandDisable(CombatController cc)
    {
        yield return new WaitForSeconds(.5f);

        cc.SetHandParticles(false);
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

            attacker = GameManager.s_instance._players[index].transform;

            //Debug.Log("Attacker set to: " + attacker.name);
        }
    }
}
