using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardsOfNorPlayer : Player
{
    [Header("Fire Pillar")]

    public InvisibleProj invisibleProj;

    public float pillarCooldown = 2;
    float elapsed;

    [Header("Jump")]

    public HitCollider jumpCollider;

    protected override void Update()
    {
        base.Update();

        if(elapsed > 0)
            elapsed -= Time.deltaTime;
    }

    protected override void Skill()
    {
        if (elapsed > 0)
            return;

        elapsed = pillarCooldown;

        base.Skill();
    }

    public void SummonProjectile()
    {
        InvisibleProj instance = Instantiate(invisibleProj, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));

        instance.caster = this;
    }

    protected override void Jump()
    {
        base.Jump();

        if (!CanMove)
            return;

        if (jumpCollider)
        {
            HitCollider instance = Instantiate(jumpCollider, transform.position, jumpCollider.transform.rotation);
            instance.SetAttacker(transform);

            Destroy(instance.gameObject, 0.5f);
        }
    }
}
