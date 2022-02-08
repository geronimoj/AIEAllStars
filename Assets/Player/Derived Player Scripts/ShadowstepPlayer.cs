using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowstepPlayer : Player
{
    [Header("Skill")]
    public Collider parryCollider;
    public Collider counterCollider;

    Collider _colInstance;
    /// <summary>
    /// Used for tracking the shadowstep is in a counter attack
    /// </summary>
    private bool _inCounterAttack = false;

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
        if (_colInstance)
            Destroy(_colInstance.gameObject);

        canMoveInt = 0;
        InvincibilityTime = 0;
    }

    public void CounterAttack()
    {
        _inCounterAttack = true;
        if (_colInstance)
            Destroy(_colInstance.gameObject);
        InvincibilityTime = 1;
        //If we are in a networked games, force the counter animation for them.
        if (NetworkManager.InRoom)
            photonView.RPC("CounterOccuredOnOtherClient", Photon.Pun.RpcTarget.Others);

        animator.SetTrigger("Counter");
    }

    [Photon.Pun.PunRPC]
    protected void CounterOccuredOnOtherClient()
    {   //If the counter occured on either client, tell the other to display it/have it occur
        //Make sure we don't re-enter the counter if we are already in/display it
        if (!_inCounterAttack)
            animator.SetTrigger("CounterNetwork");
    }

    public void CounterAttackSpawn()
    {
        _colInstance = Instantiate(counterCollider, transform.position, Quaternion.LookRotation(transform.forward * -1, Vector3.up));
        _colInstance.transform.SetParent(transform);

        StartCoroutine(DestroyColAfterDelay());
    }

    IEnumerator DestroyColAfterDelay()
    {
        yield return new WaitForSeconds(.5f);

        canMoveInt = 0;
        if (_colInstance)
            Destroy(_colInstance.gameObject);
        _inCounterAttack = false;
    }
}
