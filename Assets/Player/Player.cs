using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInput Controls;
    private int _moveInput;

    public float MaxHealth;
    private float _currentHealth;
    /// <summary>
    /// Health getter
    /// </summary>
    public float CurrentHealth => _currentHealth;

    public float MoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _moveInput = 0;
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        

    }

    protected void Move(int moveInput)
    {
        if (_moveInput + moveInput == 2 || _moveInput - moveInput == -2)
        {
            return;
        }

        _moveInput += moveInput;
    }

    protected virtual void Jump()
    {

    }

    protected virtual void Dash()
    {

    }

    protected virtual void Attack()
    {

    }

    protected virtual void Skill()
    {

    }

    private void InputUpdate()
    {
        if (Input.GetKeyDown(Controls.Left))
        {
            Move(-1);
        }
        if (Input.GetKeyDown(Controls.Right))
        {
            Move(1);
        }
        if (Input.GetKeyDown(Controls.Jump))
        {
            Jump();
        }
        if (Input.GetKeyDown(Controls.Dash))
        {
            Dash();
        }
        if (Input.GetKeyDown(Controls.Attack))
        {
            Attack();
        }
        if (Input.GetKeyDown(Controls.Skill))
        {
            Skill();
        }
    }
}
