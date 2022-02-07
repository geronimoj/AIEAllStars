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

    private void Update()
    {   //Check if the game is networked
        if (NetworkManager.InRoom && !attacker)
        {   //If this was spawned on an opponent, assign the attacker
            int i = NetworkManager.AmHost ? 1 : 0;
            //Null catch just in case
            if (GameManager.s_instance._players[i])
                attacker = GameManager.s_instance._players[i].transform;
        }
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
