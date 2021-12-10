using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowstepPlayer : Player
{
    public Collider parryCollider;

    protected override void Skill()
    {
        base.Skill();

    }

    public void StartParry()
    {
        InvincibilityTime = 10;
    }

    public void EndParry()
    {

    }
}
