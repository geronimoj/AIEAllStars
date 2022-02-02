using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RangedAttack : MonoBehaviourPun
{
    public Vector3 spawnOffset;

    public void Fire(Object projectile)
    {   //If we do not own the photon view, don't spawn projectiles
        if (NetworkManager.InRoom && !photonView.IsMine)
            return;

        GameObject proj = projectile as GameObject;

        if (!proj)
        {
            Debug.LogError("Event gave invalid parameter");
            return;
        }

        Vector3 offset = spawnOffset.x * transform.right + spawnOffset.y * transform.up + spawnOffset.z * transform.forward;

        if (NetworkManager.InRoom)
        {
            proj = PhotonNetwork.Instantiate(proj.name, transform.position + offset, Quaternion.LookRotation(transform.forward, Vector3.up));
        }
        else
            proj = Instantiate(proj, transform.position + offset, Quaternion.LookRotation(transform.forward, Vector3.up));
        proj.GetComponent<Bullet>().SetAttacker(transform);
    }
}
