using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : HitCollider
{
    public float moveSpeed = 20;

    public float lifeTime = 5;

    public Vector3 direction = Vector3.forward;

    public bool isExplosive = false;

    public HitCollider explosiveCollider = null;

    protected virtual void Start()
    {
        transform.SetParent(null, true);
        Destroy(gameObject, lifeTime);
        direction.Normalize();
    }

    protected virtual void Update()
    {
        Vector3 relativeVel = transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z;
        relativeVel *= moveSpeed * Time.deltaTime;
        //Raycast to check if we would hit the ground
        if (Physics.Raycast(transform.position, relativeVel.normalized, out RaycastHit hit, relativeVel.magnitude, LayerMask.GetMask("Ground")))
            //Clamp the distance so it stops on the ground
            relativeVel = relativeVel.normalized * hit.distance;
        //Move the collider
        transform.position -= relativeVel;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Wall") || other.CompareTag("Ground"))
            if (isExplosive && explosiveCollider)
            {
                GameObject obj = Instantiate(explosiveCollider.gameObject, transform.position, explosiveCollider.transform.rotation);
                obj.GetComponent<HitCollider>().SetAttacker(Attacker);
                Destroy(obj, 0.5f);
                isExplosive = false;
                Destroy(gameObject);
            }
    }
}
