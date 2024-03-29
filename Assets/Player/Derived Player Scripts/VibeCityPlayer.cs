﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibeCityPlayer : Player
{
    private Vector3 destination = Vector3.zero;
    private bool zipping = false;

    public float zipSpeed = 40;

    public float zipEndDist = 0.2f;

    public void ZipToTarget(Vector3 end)
    {
        destination = end;
        zipping = true;
    }

    [Photon.Pun.PunRPC]
    protected override void Skill()
    {
        if (!CanMove)
            return;

        base.Skill();
        canMoveInt++;
    }

    protected override void Update()
    {
        if (zipping)
        {
            Vector3 toDest = destination - transform.position;

            if (toDest.magnitude < zipEndDist)
            {
                zipping = false;
                canMoveInt = 0;
            }
            else
            {
                _velocity.y = 0;
                _characterController.Move(toDest.normalized * (zipSpeed * Time.deltaTime));
            }
        }

        base.Update();
    }

    public void FailGrapple()
    {
        canMoveInt--;
    }
}
