using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenPhoenixPlayer : Player
{
    public float skillJumpForce = 10;

    public float skillHorizontalForce = 1;

    protected override void Skill()
    {   //Require air charges to use since this is a jump
        if (_airCharges == 0 || !CanMove)
            return;

        //Play special animation
        animator.SetTrigger("Skill");
        //Reduce air charges
        if (!_isGrounded)
            _airCharges--;
        canMoveInt++;
        _velocity.y = skillJumpForce;
        _velocity.x = Vector3.Dot(transform.forward, Vector3.right) * skillHorizontalForce;
    }

    public void SkillEnd()
    {
        canMoveInt--;
    }
}
