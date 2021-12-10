using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardsOfNorPlayer : Player
{
    [Header("Fire Pillar")]

    public InvisibleProj invisibleProj;

    protected override void Skill()
    {
        base.Skill();


    }

    public void SummonProjectile()
    {
        InvisibleProj instance = Instantiate(invisibleProj, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));

        instance.caster = this;
    }
}
