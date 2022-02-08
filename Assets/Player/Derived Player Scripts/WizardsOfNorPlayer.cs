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

    [Photon.Pun.PunRPC]
    protected override void Skill()
    {
        if (elapsed > 0 && (photonView.IsMine || !NetworkManager.InRoom))
                return;

        elapsed = pillarCooldown;

        base.Skill();
    }

    public void SummonProjectile()
    {   //Don't let clients summon projectiles
        if (NetworkManager.InRoom && !photonView.IsMine)
            return;

        InvisibleProj instance = Instantiate(invisibleProj, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));

        instance.caster = this;
    }

    //Because most of the jump logic was put into the RPC, we don't need to override the base jump class anymore
    [Photon.Pun.PunRPC]
    public override void RPCJump()
    {
        base.RPCJump();
        //Spawn the jump collider from wizards of Nor
        if (jumpCollider)
        {
            HitCollider instance = Instantiate(jumpCollider, transform.position, jumpCollider.transform.rotation);
            instance.SetAttacker(transform);

            Destroy(instance.gameObject, 0.5f);
        }
    }
}
