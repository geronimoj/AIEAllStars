using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CombatController : MonoBehaviour
{
    Animator animator;

    public HitCollider[] attacks;

    HitCollider currentlyActiveAttack;

    bool inAttackState = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void InputAttack()
    {
        //Check if in hitstun

        if (inAttackState)
            return;

        //Change the animation to be the next attack
        animator.SetTrigger("Attack");
    }

    //When you enter the attack animation
    public void EnterAttackState()
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
        inAttackState = true;

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
    }


}
