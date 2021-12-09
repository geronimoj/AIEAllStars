using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float MoveSpeed;

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
        InputUpdate();

        _characterController.Move(Vector3.right * _moveInput * MoveSpeed * Time.deltaTime);
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
