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
        StartCoroutine(Kill());
        direction.Normalize();
    }

    protected virtual void Update()
    {
        Vector3 relativeVel = transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z;
        //Move the collider
        transform.position -= relativeVel * (moveSpeed * Time.deltaTime);
    }

    private IEnumerator Kill()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (isExplosive && explosiveCollider)
        {
            Instantiate(explosiveCollider.gameObject, transform.position, explosiveCollider.transform.rotation);
            isExplosive = false;
        }
    }
}
