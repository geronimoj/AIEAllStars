﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatController))]
public class Player : MonoBehaviour
{
    public PlayerInput Controls;

    public float MaxHealth;
    float _currentHealth;
    /// <summary>
    /// Health getter
    /// </summary>
    public float CurrentHealth => _currentHealth;

    [Space]
    public float MoveSpeed;
    public float DashMultiplier;
    public float JumpHeight = 2f;
    public float AirDashTime = 1f;
    int _moveInput;
    int _dashInput = 0;
    int _airCharges = 1;
    bool _dashing = false;

    public Transform GroundCheck;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.2f;
    public LayerMask GroundMask;
    bool _isGrounded;
    Vector3 _velocity;

    CombatController _combatController;
    CharacterController _characterController;

    Animator animator;

    private void Awake()
    {
        _combatController = GetComponent<CombatController>();
        _characterController = GetComponent<CharacterController>();
        _currentHealth = MaxHealth;

        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _moveInput = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        InputUpdate();
        Vector3 inputVelocity = Vector3.zero;

        //Change movement speed if player is dashing/Check that the player is still dashing
        if (_dashing && _isGrounded)
        {
            if (_dashInput == _moveInput)
            {
                inputVelocity = Vector3.right * _moveInput * (MoveSpeed * DashMultiplier) * Time.deltaTime;
            }
            else
            {
                _dashing = false;
                _dashInput = 0;
            }
        }
        if(_dashing && !_isGrounded)
        {
            inputVelocity = Vector3.right * _dashInput * (MoveSpeed * DashMultiplier ) * Time.deltaTime;
        }
        if(!_dashing)
        {
            inputVelocity = Vector3.right * _moveInput * MoveSpeed * Time.deltaTime;
        }

        //If moving, walk
        animator.SetFloat("MoveSpeed", inputVelocity.magnitude > 0.1f ? 1 : 0);

        _characterController.Move(inputVelocity);

        animator.SetBool("Grounded", _isGrounded);

        if (!_isGrounded)
        {
            if (!_dashing)
            {
                _velocity.y += Gravity * Time.deltaTime;
            }
        }
        else
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, Time.deltaTime * 15);
            _velocity.z = Mathf.MoveTowards(_velocity.z, 0, Time.deltaTime * 15);

            if (_airCharges != 1)
            {
                _airCharges = 1;
            }

            if (_velocity.y < 0)
            {
                _velocity.y = 0;
            }
        }

        _characterController.Move(_velocity * Time.deltaTime);

        inputVelocity = transform.position;
        inputVelocity.z = 0;
        transform.position = inputVelocity;
    }

    protected void Move(int moveInput)
    {
        if (Mathf.Abs(_moveInput + moveInput) > 1)
        {
            return;
        }

        _moveInput += moveInput;
    }

    protected virtual void Jump()
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
        else if (!_isGrounded && _airCharges > 0)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _airCharges--;
        }
    }

    /// <summary>
    /// Make the player dash, and save the direction the player was inputting when they dashed
    /// </summary>
    protected virtual void Dash()
    {
        if (_isGrounded)
        {
            _dashing = true;
        }
        else if (_airCharges > 0)
        {
            _velocity.y = 0;

            _dashing = true;
            _airCharges--;
            StartCoroutine(EndAirDash(AirDashTime));
        }

        _dashInput = _moveInput;
    }

    protected virtual void Attack()
    {
        _combatController.InputAttack();
    }

    protected virtual void Skill()
    {

    }

    public void GotHit(float damage, float stunDuration, Vector3 force)
    {
        //Take damage
        _currentHealth -= damage;

        //Get Stunned

        //Get knockedBack
        _velocity = force;
    }

    /// <summary>
    /// Checks all of the player's inputs
    /// </summary>
    private void InputUpdate()
    {
        //If the player isn't pressing either direction, make sure they aren't moving
        //Mostly a precaution
        if (Input.GetKey(Controls.Left) == false && Input.GetKey(Controls.Right) == false)
        {
            _moveInput = 0;
        }

        if (Input.GetKeyDown(Controls.Right) || Input.GetKeyUp(Controls.Left))
        {
            Move(1);
        }
        if (Input.GetKeyDown(Controls.Left) || Input.GetKeyUp(Controls.Right))
        {
            Move(-1);
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

    IEnumerator EndAirDash(float dashTime)
    {
        yield return new WaitForSeconds(dashTime);

        if (!_isGrounded && _dashing)
        {
            _dashing = false;
            _dashInput = 0;
        }
    }
}
