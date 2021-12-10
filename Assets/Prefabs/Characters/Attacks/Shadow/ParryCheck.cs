using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryCheck : MonoBehaviour
{
    protected Transform attacker;
    public void SetAttacker(Transform me)
    {
        attacker = me;
    }

    private void OnTriggerEnter(Collider other)
    {
        HitCollider _hit = other.GetComponent<HitCollider>();
    
        if (_hit)
        {
            if (_hit.Attacker == attacker)
                return;
    
            attacker.GetComponent<ShadowstepPlayer>().CounterAttack();
        }
    }
}
