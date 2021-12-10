using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowstepPlayer : Player
{
    [Header("Skill")]
    public GameObject parryCollider;

    Collider _colInstance;

    protected override void Skill()
    {
        base.Skill();
    }

    public void StartParry()
    {
        InvincibilityTime = 10;
        _colInstance = Instantiate(parryCollider, transform.position, parryCollider.transform.rotation);
    }

    public void EndParry()
    {
        InvincibilityTime = 0;
        parryCollider.SetActive(false);
    }
}
