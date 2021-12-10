﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardsOfNorPlayer : Player
{
    [Header("Fire Pillar")]

    public InvisibleProj invisibleProj;

    public float pillarCooldown = 2;
    float elapsed;

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
}
