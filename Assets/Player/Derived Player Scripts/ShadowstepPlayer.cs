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
        canMoveInt++;
        InvincibilityTime = 10;
        _colInstance = Instantiate(parryCollider.GetComponent<Collider>(), transform.position, parryCollider.transform.rotation);
    }

    public void EndParry()
    {
        InvincibilityTime = 0;
        canMoveInt--;

        if (_colInstance)
            Destroy(_colInstance.gameObject);
    }
}
