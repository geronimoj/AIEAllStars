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
        InvincibilityTime = 10;
        _colInstance = Instantiate(parryCollider, transform.position, parryCollider.transform.rotation);
        canMoveInt++;

        _colInstance.GetComponent<ParryCheck>().SetAttacker(gameObject.transform);
        _colInstance.transform.SetParent(transform);
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
    }

    public void CounterAttackSpawn()
    {
        _colInstance = Instantiate(counterCollider, transform.position, counterCollider.transform.rotation);
        _colInstance.transform.SetParent(transform);

        StartCoroutine(DestroyColAfterDelay());
    }

    IEnumerator DestroyColAfterDelay()
    {
        yield return new WaitForSeconds(.5f);

        if (_colInstance)
            Destroy(_colInstance.gameObject);
    }
}
