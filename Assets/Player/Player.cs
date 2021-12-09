using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatController))]
public class Player : MonoBehaviour
{
    public PlayerInput Controls;
    int _moveInput;

    public float MaxHealth;
    float _currentHealth;
    /// <summary>
    /// Health getter
    /// </summary>
    public float CurrentHealth => _currentHealth;

    [Space]
    public float MoveSpeed;
    public float Gravity = -9.81f;
    public float JumpHeight = 2f;
    int _airCharges = 1;
    bool _isGrounded;
    public Transform GroundCheck;
    public float GroundDistance = 0.2f;
    public LayerMask GroundMask;
    Vector3 _velocity;

    CombatController _combatController;
    CharacterController _characterController;

    private void Awake()
    {
        _combatController = GetComponent<CombatController>();
        _characterController = GetComponent<CharacterController>();
        _currentHealth = MaxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        _moveInput = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        InputUpdate();

        _characterController.Move(Vector3.right * _moveInput * MoveSpeed * Time.deltaTime);

        if (!_isGrounded)
        {
            _velocity.y += Gravity * Time.deltaTime;
        }
        else
        {
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

    protected virtual void Dash()
    {

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

        //Get Stunned

        //Get knockedBack
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
}
