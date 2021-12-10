using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : HitCollider
{
    public float moveSpeed = 20;

    public float lifeTime = 5;

    private void Start()
    {
        transform.SetParent(null, true);
        StartCoroutine(Kill());
    }

    private void Update()
    {   //Move the collider
        transform.position -= transform.forward * (moveSpeed * Time.deltaTime);
    }

    private IEnumerator Kill()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
