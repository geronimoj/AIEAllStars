using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NorSpecial : StateMachineBehaviour
{
    Player p;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        p = animator.GetComponent<Player>();

        if (p)
        {
            p.canMoveInt++;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (p)
        {
            p.canMoveInt--;
            animator.ResetTrigger("Skill");
        }
    }
}
