using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class vfxObj : MonoBehaviour
{
    ParticleSystem ps;

    public float lifeTime;
    float elapsed;

    public bool follow;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        if(elapsed > lifeTime)
        {
            EndEffect();
        }
    }

    public void Initialise()
    {
        ps.Play();
    }

    public void EndEffect()
    {
        ps.enableEmission = false;

        Destroy(gameObject, 5);
    }
}
