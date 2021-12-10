using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaemSpecial : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player p = animator.GetComponent<Player>();

        if (p)
        {
            p.InvincibilityTime = 0.5f;
        }
    }
}
