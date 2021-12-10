using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowstepPlayer : Player
{
    public GameObject parryCollider;

    private void Start()
    {
        parryCollider.SetActive(false);
    }

    protected override void Skill()
    {
        base.Skill();
    }

    public void StartParry()
    {
        InvincibilityTime = 10;
        parryCollider.SetActive(true);
    }

    public void EndParry()
    {
        InvincibilityTime = 0;
        parryCollider.SetActive(false);
    }
}
