using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Player))]
public class CombatController : MonoBehaviour
{
    //on first attack input, will do single light
    //when second pressed, attack will queue for x seconds
    //if nothing is queued when attack ends, enter selfStun
    //if something queued, unqueue it and start next attack

    Player player;

    Animator animator;

    public HitCollider[] attacks;

    HitCollider currentlyActiveAttack;

    public bool inAttackState = false;

    float queueTimer = 0; //goes to one when queued

    float attackCooldown = 0; // when greater than zero, cannot attack


    [Header("Other")]
    [Tooltip("optional point")]
    public Transform weaponPoint;

    [Tooltip("Particles which only emit when attacking")]
    public ParticleSystem[] attackParticles;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();

        inAttackState = false;
        queueTimer = 0;

        SetHandParticles(false);
    }

    [ContextMenu("Attack")]
    public void InputAttack()
    {
        //Check if in hitstun
        //If we are already attacking and want to queue another
        if (inAttackState || player.CanMove == false)
        {
            queueTimer = 1;
            return;
        }

        if (attackCooldown > 0)
            return;

        inAttackState = true;

        //Change the animation to be the next attack
        animator.SetTrigger("Attack");

        player.StunForDuration(0.3f);
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
        WizardsOfNorPlayer p = player as WizardsOfNorPlayer;
        if(p)
            SetHandParticles(true);

        //If we can make a valid attack
        if (attacks != null)
            if (attacks.Length > attack)
                if (attacks[attack] != null)
                {
                    DestroyActiveCol();

                    currentlyActiveAttack = Instantiate(attacks[attack], transform.position, Quaternion.LookRotation(transform.forward*-1, Vector3.up));

                    if(currentlyActiveAttack.followUser)
                        currentlyActiveAttack.transform.SetParent(transform);

                    currentlyActiveAttack.SetAttacker(transform);
                }

    }

    //The part of the animation where the collider destroys
    public void EndAttack(int attack)
    {
        inAttackState = false;

        WizardsOfNorPlayer p = player as WizardsOfNorPlayer;
        if (p)
            SetHandParticles(false);

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

        DestroyActiveCol();

        if (attack == 2)
            FinishAttackChain();
    }

    public void DestroyActiveCol()
    {
        if (currentlyActiveAttack)
        {
            Bullet b = currentlyActiveAttack as Bullet;
            if(!b)
                Destroy(currentlyActiveAttack.gameObject);
        }
    }

    public void FinishAttackChain()
    {

    }

    public void SetHandParticles(bool state)
    {
        if(attackParticles != null)
            foreach (ParticleSystem ps in attackParticles)
            {
                if (ps)
                {
                    ps.enableEmission = state;
                }
            }
    }
}
