using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleProj : MonoBehaviour
{
    public float lifeTime;
    float elapsed = 0;
    public float speed;

    public FirePillar firePillar;

    [HideInInspector]
    public Player caster;

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed > lifeTime)
            SummonPillar();

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
            return;

        //If we hit an enemy or wall, summon a fire pillar here

        Player p = other.GetComponent<Player>();
        if (p)
        {
            if (p == caster)
                return;

            SummonPillar();
        }
        else
        {
            if (other.CompareTag("Wall"))
                SummonPillar();
        }
    }

    void SummonPillar()
    {
        FirePillar instance = Instantiate(firePillar, transform.position, firePillar.transform.rotation);



        Destroy(gameObject);
    }
}
