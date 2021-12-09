using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThePoundPlayer : Player
{
    private float _minSpeed = 0;

    public float _maxSpeedAt0HP = 15;

    protected override void Start()
    {
        base.Start();
        _minSpeed = MoveSpeed;
    }

    protected override void Update()
    {   //Scale move speed with health
        MoveSpeed = Mathf.Lerp(_maxSpeedAt0HP, _minSpeed, CurrentHealth / MaxHealth);
        //Base update
        base.Update();
    }
}
