﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatController))]
public class Player : MonoBehaviour
{
    #region Variables
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
    public float AirDashBoost = 2f;
    public int MaxAirActions = 1;
    int _airCharges = 1;
    int _moveInput;
    int _dashInput = 0;
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

    [Header("AI Customisation")]

    [Space]
    public bool IsAI = true;
    protected int MoveInput = 0;

    [Space]
    public TimerData MoveData;
    public TimerData JumpData;
    public TimerData AttackData;
    public TimerData DashData;
    public TimerData SkillData;

    [System.Serializable]
    public class TimerData
    {
        public float MinTime = 0;
        public float MaxTime = 1;
        protected float CurrentTime = 0;

        public bool CallTimer()
        {
            if (CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
                return false;
            }
            else
            {
                ResetTimer();
                return true;
            }
        }

        public void ResetTimer()
        {
            CurrentTime = Random.Range(MinTime, MaxTime);
        }
    }

    public int canMoveInt = 0;
    public bool CanMove => canMoveInt <= 0;
    #endregion

    #region Start/Update
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
        //Check if the player is grounded
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (!IsAI)
            InputUpdate();
        else
            AIUpdate();

        Vector3 inputVelocity = Vector3.zero;

        //Change movement speed if player is dashing/Check that the player is still dashing
        if (_dashing && _isGrounded)
        {
            //If you're still pressing the same direction, keep dashing
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
        //Dashing in mid-air
        if(_dashing && !_isGrounded)
        {
            inputVelocity = Vector3.right * _dashInput * (MoveSpeed * DashMultiplier * AirDashBoost) * Time.deltaTime;
        }
        //Regular movement speed
        if(!_dashing)
        {
            inputVelocity = Vector3.right * _moveInput * MoveSpeed * Time.deltaTime;
        }

        //If moving, walk
        animator.SetFloat("MoveSpeed", _moveInput != 0 ? 1 : 0);

        if (!CanMove)
            inputVelocity = Vector3.zero;

        //Move the character controller based on the player's input
        _characterController.Move(inputVelocity);

        animator.SetBool("Grounded", _isGrounded);

        if (!_isGrounded)
        {
            if (!_dashing)
            {
                //If not grounded or air dashing, apply gravity
                _velocity.y += Gravity * Time.deltaTime;
            }
        }
        //If you are grounded...
        else
        {
            //Used to prevent knockback sliding + sticking to the floor during knockback
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, Time.deltaTime * 15);
            _velocity.z = Mathf.MoveTowards(_velocity.z, 0, Time.deltaTime * 15);

            //Reset midair actions
            if (_airCharges != 1)
            {
                _airCharges = MaxAirActions;
            }

            //Reset gravity's changes to velocity
            if (_velocity.y < 0)
            {
                _velocity.y = 0;
            }
        }

        //Move downwards with their increased gravity
        _characterController.Move(_velocity * Time.deltaTime);

        //Clamps the player to z = 0
        inputVelocity = transform.position;
        inputVelocity.z = 0;
        transform.position = inputVelocity;
    }
    #endregion

    #region Actions
    protected void Move()
    {
        int moveInput = 0;

        if(Input.GetKey(Controls.Right))
        {
            moveInput++;
        }
        if(Input.GetKey(Controls.Left))
        {
            moveInput--;
        }

        switch (moveInput)
        {
            case -1:
                FaceLeft();
                break;

            case 0:
                FaceEnemy();
                break;

            case 1:
                FaceRight();
                break;
        }

        _moveInput = moveInput;
    }

    protected virtual void Jump()
    {
        _dashing = false;

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
        animator.SetTrigger("Dash");

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

        animator.SetTrigger("Hit");
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
            FaceEnemy();
        }

        if (Input.GetKey(Controls.Right) || Input.GetKey(Controls.Left))
        {
            Move();
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
            FaceEnemy();
            Attack();
        }
        if (Input.GetKeyDown(Controls.Skill))
        {
            FaceEnemy();
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
    #endregion

    #region AI
    protected virtual bool AISkillCanBeUsed()
    {
        //Need to override in each different character. This is the trigger for each different character to use the skill. It also has to line up with the timer hitting 0.
        //some characters wont need triggers and therefore will just have return true
        //example trigger

        int trigger = Random.Range(0, 2);

        if (trigger == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void AIAttack()
    {
        FaceEnemy();
        Attack();
    }

    private void AIUpdate()
    {
        _moveInput = MoveInput;

        if (CallTimer(MoveData))
        {
            ChooseMoveDirection();
        }

        if (CallTimer(AttackData))
        {
            AIAttack();
        }

        if (CallTimer(JumpData))
        {
            Jump();
        }

        if (CallTimer(DashData))
        {
            Dash();
        }

        if (CallTimer(SkillData))
        {
            if (AISkillCanBeUsed())
            {
                Skill();
            }
        }
    }

    protected void ChooseMoveDirection()
    {
        if(EnemyIsOnLeft())
        {
            MoveInput = Random.Range(-2, 2);

            if(MoveInput < -1)
            {
                MoveInput = -1;
            }
        }
        else
        {
            MoveInput = Random.Range(-1, 3);

            if(MoveInput > 1)
            {
                MoveInput = 1;
            }
        }
    }
    #endregion

    #region Misc
    public bool CallTimer(TimerData Timer)
    {
        if (!Timer.CallTimer())
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected GameObject Enemy()
    {
        if (gameObject == GameManager.s_instance._players[0].gameObject)
        {
            return GameManager.s_instance._players[1].gameObject;
        }
        else
        {
            return GameManager.s_instance._players[0].gameObject;
        }
    }

    protected bool EnemyIsOnLeft()
    {
        if (Enemy().transform.position.x > transform.position.x)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected void FaceEnemy()
    {
        if (EnemyIsOnLeft())
        {
            FaceLeft();
        }
        else
        {
            FaceRight();
        }
    }

    protected void FaceRight()
    {
        transform.eulerAngles = new Vector3(0, -90, 0);
    }

    protected void FaceLeft()
    {
        transform.eulerAngles = new Vector3(0, 90, 0);
    }
    #endregion
}
