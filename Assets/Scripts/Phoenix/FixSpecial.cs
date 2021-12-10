using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixSpecial : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GoldenPhoenixPlayer vb = animator.GetComponent<GoldenPhoenixPlayer>();

        vb.SkillEnd();
    }
}
