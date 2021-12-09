using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitCollider : MonoBehaviour
{
    enum Follow
    {
        none,
        torso,
        weapon,
        collider,
    }

    [Tooltip("damage this attack will do")]
    public float damage;

    [Tooltip("how long it will stun the person hit for")]
    public float enemyStunDuration;

    [Tooltip("if another input isn't pressed, you will be unable to attack again for this amount of time")]
    public float selfStunDuration;

    [Tooltip("0 is forwards relative to the character forwards, 90 is up")]
    public float launchAngle;

    public float launchForce;

    public bool followUser = true;

    public bool giveInvFrames = false;

    [Header("FX")]
    public vfxObj particles;
    [SerializeField] Follow toFollow;
    vfxObj partInstance;

    Transform attacker;
    public void SetAttacker(Transform me)
    {
        attacker = me;
    }

    private void Awake()
    {
        if (!particles)
            return;

        partInstance = Instantiate(particles, transform.position + particles.transform.position, particles.transform.rotation);

        switch (toFollow)
        {
            case Follow.none:
                break;
            case Follow.torso:
                partInstance.transform.SetParent(attacker);
                break;
            case Follow.weapon:
                CombatController cc = attacker.GetComponent<CombatController>();
                if (cc)
                {
                    partInstance.transform.SetParent(cc.weaponPoint);
                }
                break;
            case Follow.collider:
                partInstance.transform.SetParent(transform);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if the other is a character, and if so hit them
        Player p = other.GetComponent<Player>();

        if (p)
        {
            if (p.transform == attacker)
                return;

            if (p.invinsibilityTime > 0)
                return;

            //Calculate laucnh force
            Vector3 angle = transform.forward * Mathf.Sin(launchAngle) + transform.up * Mathf.Cos(launchAngle);
            angle.Normalize();

            p.GotHit(damage, enemyStunDuration, angle * launchForce);

            if (giveInvFrames)
            {
                //Make enemy invinsible for short time
                p.invinsibilityTime = 0.5f;
            }
        }
    }
}
