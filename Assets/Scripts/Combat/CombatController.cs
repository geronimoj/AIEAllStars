﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CombatController : MonoBehaviour
{
    //on first attack input, will do single light
    //when second pressed, attack will queue for x seconds
    //if nothing is queued when attack ends, enter selfStun
    //if something queued, unqueue it and start next attack



    Animator animator;

    public HitCollider[] attacks;

    HitCollider currentlyActiveAttack;

    bool inAttackState = false;

    float queueTimer = 0; //goes to one when queued

    private void Awake()
    {
        animator = GetComponent<Animator>();

        inAttackState = false;
        queueTimer = 0;
    }

    [ContextMenu("Attack")]
    public void InputAttack()
    {
        //Check if in hitstun


        //If we are already attacking and want to queue another
        if (inAttackState)
        {
            queueTimer = 1;
            return;
        }

        inAttackState = true;

        //Change the animation to be the next attack
        animator.SetTrigger("Attack");
    }

    private void Update()
    {
        if(queueTimer > 0)
            queueTimer -= Time.deltaTime;
    }

    //When you enter the attack animation
    public void EnterAttackState(int attack)
    {

    }

    //When you leave the attack animation
    public void ExitAttackState()
    {
        EndAttack();
    }

    //The part in the animation where the collider spawns
    public void StartAttack(int attack)
    {
        //If we can make a valid attack
        if (attacks != null)
            if (attacks.Length > attack)
                if (attacks[attack] != null)
                {
                    currentlyActiveAttack = Instantiate(attacks[attack], transform.position, attacks[attack].transform.rotation * Quaternion.Euler(transform.forward));
                }
    }

    //The part of the animation where the collider destroys
    public void EndAttack()
    {
        inAttackState = false;

        if (currentlyActiveAttack)
        {
            Destroy(currentlyActiveAttack.gameObject);
        }

        if(queueTimer > 0)
        {
            queueTimer = 0;

            InputAttack();
        }
    }
}