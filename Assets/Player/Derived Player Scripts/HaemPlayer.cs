using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaemPlayer : Player
{
    [Header("Skill")]
    public HitCollider specialCollider;

    HitCollider colInstance;

    protected override void Skill()
    {
        base.Skill();

        GotHit(7, 0, Vector3.zero, false);

        //summon a shockwave
        colInstance = Instantiate(specialCollider, transform.position, specialCollider.transform.rotation);
        colInstance.SetAttacker(transform);

        StartCoroutine(DestroyColAfterDelay());
    }

    IEnumerator DestroyColAfterDelay()
    {
        yield return new WaitForSeconds(.5f);

        if (colInstance)
            Destroy(colInstance.gameObject);
    }
}
