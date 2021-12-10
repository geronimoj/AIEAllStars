using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaemPlayer : Player
{
    [Header("Skill")]
    public HitCollider specialCollider;

    HitCollider colInstance;

    public vfxObj shockWave;
    public vfxObj quake;

    protected override void Skill()
    {
        //base.Skill();

        if (CurrentHealth < 8)
            return;

        animator.SetTrigger("Skill");
    }

    public void ShockWave()
    {
        GotHit(7, 0, Vector3.zero, false);

        //summon a shockwave
        colInstance = Instantiate(specialCollider, transform.position, specialCollider.transform.rotation);
        colInstance.SetAttacker(transform);

        StartCoroutine(DestroyColAfterDelay());

        vfxObj instance = null;

        if (!_isGrounded)
            instance = Instantiate(quake, transform.position + quake.transform.position, quake.transform.rotation);
        else
            instance = Instantiate(shockWave, transform.position + shockWave.transform.position, shockWave.transform.rotation);

        instance.Initialise();
    }

    IEnumerator DestroyColAfterDelay()
    {
        yield return new WaitForSeconds(.5f);

        if (colInstance)
            Destroy(colInstance.gameObject);
    }
}
