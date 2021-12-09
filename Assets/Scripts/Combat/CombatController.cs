using System.Collections;
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

    public bool inAttackState = false;

    float queueTimer = 0; //goes to one when queued

    float attackCooldown = 0; // when greater than zero, cannot attack


    [Header("Other")]
    [Tooltip("optional point")]
    public Transform weaponPoint;

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

        if (attackCooldown > 0)
            return;

        inAttackState = true;

        //Change the animation to be the next attack
        animator.SetTrigger("Attack");
    }

    private void Update()
    {
        if(queueTimer > 0)
            queueTimer -= Time.deltaTime;

        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
    }

    //When you enter the attack animation
    public void EnterAttackState()
    {

    }

    //When you leave the attack animation
    public void ExitAttackState()
    {
        EndAttack(0);
    }

    //The part in the animation where the collider spawns
    public void StartAttack(int attack)
    {
        //If we can make a valid attack
        if (attacks != null)
            if (attacks.Length > attack)
                if (attacks[attack] != null)
                {
                    if (currentlyActiveAttack)
                        Destroy(currentlyActiveAttack.gameObject);

                    currentlyActiveAttack = Instantiate(attacks[attack], transform.position, Quaternion.LookRotation(transform.forward*-1, Vector3.up));

                    currentlyActiveAttack.SetAttacker(transform);
                }
    }

    //The part of the animation where the collider destroys
    public void EndAttack(int attack)
    {
        inAttackState = false;

        if (queueTimer > 0)
        {
            queueTimer = 0;

            InputAttack();
        }
        else
        {
            //Don't let the player input attacks for x seconds
            if (currentlyActiveAttack)
                attackCooldown = currentlyActiveAttack.selfStunDuration;
        }

        if (currentlyActiveAttack)
            Destroy(currentlyActiveAttack.gameObject);

        if (attack == 2)
            FinishAttackChain();
    }

    public void FinishAttackChain()
    {

    }
}
