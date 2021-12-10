using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowstepPlayer : Player
{
    [Header("Skill")]
    public Collider parryCollider;
    public Collider counterCollider;

    Collider _colInstance;

    protected override void Skill()
    {
        base.Skill();
    }

    public void StartParry()
    {
        canMoveInt++;
        InvincibilityTime = 10;
        _colInstance = Instantiate(parryCollider, transform.position, parryCollider.transform.rotation);
    }

    public void EndParry()
    {
        InvincibilityTime = 0;
        canMoveInt--;

        if (_colInstance)
            Destroy(_colInstance.gameObject);
    }

    public void CounterAttack()
    {
        if (_colInstance)
            Destroy(_colInstance.gameObject);
        InvincibilityTime = 1;

        animator.SetTrigger("Counter");
        _colInstance = Instantiate(counterCollider, transform.position, counterCollider.transform.rotation);
    }
}
