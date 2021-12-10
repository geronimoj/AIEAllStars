using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThePoundPlayer : Player
{
    private float _minSpeed = 0;

    private Renderer r = null;

    public float _maxSpeedAt0HP = 15;

    protected override void Start()
    {
        base.Start();
        _minSpeed = MoveSpeed;

        r = GetComponentInChildren<Renderer>();
        Material mat = Instantiate(r.material);
        r.material = mat;
    }

    protected override void Update()
    {   //Scale move speed with health
        MoveSpeed = Mathf.Lerp(_maxSpeedAt0HP, _minSpeed, CurrentHealth / MaxHealth);
        r.material.SetFloat("_Amount", CurrentHealth / MaxHealth);

        //Base update
        base.Update();
    }
}
