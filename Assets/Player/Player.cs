﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatController))]
public class Player : MonoBehaviourPun, IPunObservable, IPlayerRollback
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
    protected int _airCharges = 1;
    int _moveInput;
    int _dashInput = 0;
    bool _dashing = false;

    public float Gravity = -9.81f;
    public float GroundDistance = 0.2f;
    public Transform GroundCheck;
    public LayerMask GroundMask;
    ///// <summary>
    ///// The distance between you can be above a character without being pushed back
    ///// </summary>
    //public float CharacterDistance = 0.5f;
    //public Transform CharacterCheck;
    //public LayerMask CharacterMask;

    protected bool _isGrounded;
    protected Vector3 _velocity;

    CombatController _combatController;
    protected CharacterController _characterController;

    protected Animator animator;

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

    public float InvincibilityTime = 0;
    /// <summary>
    /// The radius of the player in world size
    /// </summary>
    public float Radius => _characterController.radius * transform.localScale.x;

    SkinnedMeshRenderer mesh;
    Material invincibleGlow;

    [Header("VFX")]
    public vfxObj hitParticles;
    public vfxObj dashParticles;
    public vfxObj jumpParticles;

    private bool _inputChange = false;
    private bool _simulationMode = false;
    private InputInfo _inputInfo = default;

    float _dashTimer = 0f;
    #endregion

    public bool enableDebug = false;

    #region Start/Update
    private void Awake()
    {
        _combatController = GetComponent<CombatController>();
        _characterController = GetComponent<CharacterController>();
        _currentHealth = MaxHealth;

        animator = GetComponent<Animator>();

        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        invincibleGlow = Resources.Load<Material>("WhiteGlow") as Material;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _moveInput = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Material[] matArray = mesh.materials;
        if (InvincibilityTime > 0)
        {
            InvincibilityTime -= Time.deltaTime;

            if (invincibleGlow)
            {
                matArray[1] = invincibleGlow;
            }
        }
        else
        {
            matArray[1] = null;
        }
        mesh.materials = matArray;

        SimulateMove(Time.deltaTime);
    }

    protected virtual void LateUpdate()
    {
        if (!GameManager.s_instance)
            return;
        //Only perform in networked lobby
        //We do this in LateUpdate to make sure all inputs from this frame have been recieved
        if (NetworkManager.InRoom && _inputChange)
        {
            _inputChange = false;
            photonView.RPC("RPCRollback", RpcTarget.All, GameManager.GameTime, _inputInfo.ToByte());
            //Disable all boolean inputs
            _inputInfo.dash = false;
            _inputInfo.jump = false;
            _inputInfo.attack = false;
            _inputInfo.skill = false;
        }
    }
    #endregion

    #region Simulation
    protected virtual void SimulateMove(float deltaTime)
    {
        bool prev = _isGrounded;
        //Check if the player is grounded
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (prev == true && _isGrounded == false)
            _dashTimer = 0f;
        //Don't update inputs in simulation mode
        if (!_simulationMode)
        {
            if (!IsAI)
                InputUpdate();
            else
                AIUpdate();
        }

        Vector3 inputVelocity = Vector3.zero;
        //Check if dash timer is finished.
        _dashTimer -= deltaTime;

        if (_dashTimer <= 0)
            _dashInput = 0;

        _dashing = _dashInput != 0 && (_dashTimer > 0f || _isGrounded);

        if (CanMove)
        {
            //Change movement speed if player is dashing/Check that the player is still dashing
            if (_dashing && _isGrounded)
            {
                //If you're still pressing the same direction, keep dashing
                if (_dashInput == _moveInput)
                {
                    inputVelocity = Vector3.right * (_moveInput * (MoveSpeed * DashMultiplier) * deltaTime);
                }
                else
                {
                    _dashing = false;
                    _dashInput = 0;
                }
            }
            //Dashing in mid-air
            if (_dashing && !_isGrounded)
            {
                inputVelocity = Vector3.right * (_dashInput * (MoveSpeed * DashMultiplier * AirDashBoost) * deltaTime);
            }
            //Regular movement speed
            if (!_dashing)
            {
                inputVelocity = Vector3.right * (_moveInput * MoveSpeed * deltaTime);

                if (enableDebug)
                    Debug.Log("Setting inputVelocity");
            }
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
                _velocity.y += Gravity * deltaTime;

                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deltaTime * 5);
                _velocity.z = Mathf.MoveTowards(_velocity.z, 0, deltaTime * 5);
            }
        }
        //If you are grounded...
        else
        {
            //Used to prevent knockback sliding + sticking to the floor during knockback
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deltaTime * 25);
            _velocity.z = Mathf.MoveTowards(_velocity.z, 0, deltaTime * 25);

            //Reset midair actions
            if (_airCharges != MaxAirActions)
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
        _characterController.Move(_velocity * deltaTime);
        SlideOffHead(deltaTime);

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

        if (CanMove)
        {
            if (Input.GetKey(Controls.Right))
            {
                moveInput++;
            }
            if (Input.GetKey(Controls.Left))
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
        }
        //moveInput has changed
        if (moveInput != _moveInput)
        {   //Input has changed
            _inputInfo.MoveInput = moveInput;
            _inputChange = true;
        }

        _moveInput = moveInput;
    }

    protected virtual void Jump()
    {
        if (!CanMove)
            return;

        _dashTimer = 0f;
        _dashInput = 0;
        _inputInfo.jump = true;
        _inputChange = true;

        if (_isGrounded)
        {
            RPCJump();

            /*if (NetworkManager.InRoom)
                photonView.RPC("RPCJump", RpcTarget.Others);*/
        }
        else if (!_isGrounded && _airCharges > 0)
        {
            RPCJump();
            _airCharges--;

            /*if (NetworkManager.InRoom)
                photonView.RPC("RPCJump", RpcTarget.Others);*/
        }
    }

    public virtual void RPCJump()
    {   //Set jump velocity
        _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        //Do particle stuff
        if (jumpParticles && !_simulationMode)
        {
            vfxObj instance = Instantiate(jumpParticles, transform.position + jumpParticles.transform.position, jumpParticles.transform.rotation);
            instance.Initialise();
        }
    }

    /// <summary>
    /// Make the player dash, and save the direction the player was inputting when they dashed
    /// </summary>
    protected virtual void Dash()
    {
        if (!CanMove || _moveInput == 0)
            return;

        if (_isGrounded)
        {   //I moved the dash code into the rpc to reduce copy paste
            RPCDash(false, _moveInput);
            //Tell other player about dash
            /*if (NetworkManager.InRoom)
                photonView.RPC("RPCDash", RpcTarget.Others, false, _moveInput);*/
        }
        else if (_airCharges > 0)
        {   //Reduce air charges
            _airCharges--;
            //I moved the dash code into the rpc to reduce copy paste
            RPCDash(true, _moveInput);
            //Tell other player about dash
            /*if (NetworkManager.InRoom)
                photonView.RPC("RPCDash", RpcTarget.Others, true, _moveInput);*/
        }

        _dashInput = _moveInput;
    }

    public void RPCDash(bool airborne, int dashInput)
    {
        //_dashInput = dashInput;
        _velocity.y = 0;

        if (airborne)
        {
            animator.SetTrigger("Dash");
            //StartCoroutine(EndAirDash(AirDashTime));
        }
        //Spawn particle effects
        if (dashParticles)
        {
            vfxObj instance = Instantiate(dashParticles, transform.position + dashParticles.transform.position, transform.rotation);
            instance.Initialise();
        }
        //If we received this from the other player, update the move input to match the dash input
        /*if (NetworkManager.InRoom)
        {   //For sync purposes
            _dashInput = dashInput;
            _moveInput = dashInput;
        }*/
    }

    protected virtual void Attack()
    {
        _combatController.InputAttack();
        //If its networked and we own this character, tell the other player to play an attack animation
        /*if (NetworkManager.InRoom && photonView.IsMine)
            photonView.RPC("Attack", RpcTarget.Others);*/
    }

    protected virtual void Skill()
    {
        if (!CanMove)
            return;

        animator.SetTrigger("Skill");
        //If its networked and we own this character, tell the other player to play an attack animation
        /*if (NetworkManager.InRoom && photonView.IsMine)
            photonView.RPC("Skill", RpcTarget.Others);*/
    }

    public void GotHit(float damage, float stunDuration, Vector3 force, bool playAnim = true)
    {
        //Take damage
        _currentHealth -= damage;

        //Get Stunned
        StunForDuration(stunDuration);

        //Get knockedBack
        _velocity = force;

        if (playAnim)
            animator.SetTrigger("Hit");

        if (hitParticles)
        {
            vfxObj instance = Instantiate(hitParticles, transform.position + hitParticles.transform.position, jumpParticles.transform.rotation);
            instance.Initialise();
        }
    }

    public void StunForDuration(float stunDuration)
    {
        canMoveInt++;

        _dashInput = 0;

        StartCoroutine(WaitBeforeUnStun(stunDuration));
    }

    IEnumerator WaitBeforeUnStun(float time)
    {
        yield return new WaitForSeconds(time);

        canMoveInt--;
    }

    /// <summary>
    /// Checks all of the player's inputs
    /// </summary>
    private void InputUpdate()
    {
        if (Controls != null)
        {
            //If the player isn't pressing either direction, make sure they aren't moving
            //Mostly a precaution
            if (Input.GetKey(Controls.Left) == false && Input.GetKey(Controls.Right) == false)
            {
                if (_moveInput != 0)
                {
                    _inputInfo.MoveInput = 0;
                    _inputChange = true;
                }

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
    }

    /*IEnumerator EndAirDash(float dashTime)
    {
        yield return new WaitForSeconds(dashTime);

        if (!_isGrounded && _dashing)
        {
            _dashing = false;
            _dashInput = 0;
        }
    }*/
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
        if (EnemyIsOnLeft())
        {
            MoveInput = Random.Range(-2, 2);

            if (MoveInput < -1)
            {
                MoveInput = -1;
            }
        }
        else
        {
            MoveInput = Random.Range(-1, 3);

            if (MoveInput > 1)
            {
                MoveInput = 1;
            }
        }
    }
    #endregion

    #region Misc
    public void Heal(float amount)
    {
        _currentHealth += amount;

        if (_currentHealth > MaxHealth)
            _currentHealth = MaxHealth;
    }

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
        if (GameManager.s_instance != null)
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
        return null;
    }

    protected bool EnemyIsOnLeft()
    {
        if (Enemy() != null)
        {
            if (Enemy().transform.position.x > transform.position.x)
            {
                if (transform.localScale.x != 30)
                {
                    transform.localScale = new Vector3(30, transform.localScale.y, transform.localScale.z);
                }
                return false;
            }
            else
            {
                if (transform.localScale.x != -30)
                {
                    transform.localScale = new Vector3(-30, transform.localScale.y, transform.localScale.z);
                }
                return true;
            }
        }
        return false;
    }

    protected void FaceEnemy()
    {
        if (Enemy() != null)
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
    }

    protected void FaceRight()
    {
        transform.eulerAngles = new Vector3(0, -90, 0);
    }

    protected void FaceLeft()
    {
        transform.eulerAngles = new Vector3(0, 90, 0);
    }

    void SlideOffHead(float deltaTime)
    {
        if (Enemy() != null)
        {
            Vector3 _ePos = Enemy().transform.position;
            Vector3 _pPos = transform.position;
            float _eGrav = Enemy().GetComponent<Player>()._velocity.y;
            float _pGrav = _velocity.y;

            //If the player's are too close and above one another
            if (Mathf.Abs(_ePos.x - _pPos.x) <= 1.3 && Mathf.Abs(_ePos.y - _pPos.y) <= 2)
            {
                if (transform.position.x == Enemy().transform.position.x)
                {
                    if (transform.position.x > 0)
                    {
                        //Is the enemy above you?
                        if (_ePos.y > _pPos.y)
                        {
                            _characterController.Move(new Vector3(_eGrav / 2 * deltaTime, 0, 0));
                        }
                        else
                        {
                            _characterController.Move(new Vector3(_pGrav * deltaTime, 0, 0));
                        }
                    }
                    else
                    {
                        //Is the enemy above you?
                        if (_ePos.y > _pPos.y)
                        {
                            _characterController.Move(new Vector3((_eGrav * -1) / 2 * deltaTime, 0, 0));
                        }
                        else
                        {
                            _characterController.Move(new Vector3((_pGrav * -1) * deltaTime, 0, 0));
                        }
                    }
                }
                else if (EnemyIsOnLeft())
                {
                    //Is the enemy above you?
                    if (_ePos.y > _pPos.y)
                    {
                        _characterController.Move(new Vector3((_eGrav * -1) / 2 * deltaTime, 0, 0));
                    }
                    else
                    {
                        _characterController.Move(new Vector3((_pGrav * -1) * deltaTime, 0, 0));
                    }
                }
                else
                {
                    //Is the enemy above you?
                    if (_ePos.y > _pPos.y)
                    {
                        _characterController.Move(new Vector3(_eGrav / 2 * deltaTime, 0, 0));
                    }
                    else
                    {
                        _characterController.Move(new Vector3(_pGrav * deltaTime, 0, 0));
                    }
                }
            }
        }
    }
    #endregion

    #region Photon

    /*[Header("Networking")]
    [SerializeField]
    [Tooltip("The maximum amount of time that we assume between a package being sent and recieved. Used for determining position resyc")]
    private float _maxPackageDesync = 0.1f;

    [SerializeField]
    [Tooltip("How long it takes to resync the position of the character")]
    private float _positionResyncRate = 0.1f;*/

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Keeping this just in case derived classes want to send unimportant information about themself
        //that cannot be synced through rollback
    }

    /*private IEnumerator ResyncPosition(Vector3 posDif)
    {
        Vector3 remaining = posDif;
        //Use a bit of memory to store and avoid additional calculations
        float multiplier = 1 / _positionResyncRate;
        while (remaining != Vector3.zero)
        {   //Calculate how far to move this update
            //This multiplier method isn't perfectly accurate but it will do
            Vector3 difApplied = posDif * (Time.deltaTime * multiplier);
            //If we are going to overshoot the remaining distance, clamp
            if (difApplied.magnitude > remaining.magnitude)
            {   //No more remaining distance to travel
                remaining = Vector3.zero;
                //Clamp value
                difApplied = remaining;
            }
            else//Otherwise reduce remaining
                remaining -= difApplied;
            //Apply the position change
            transform.position += difApplied;
            //We can exit early to save a worthless check again later
            if (remaining == Vector3.zero)
                break;
            //Wait a frame
            yield return null;
        }
    }*/
    #endregion

    #region Rollback
    public void SetState(BehaviourState state)
    {
        CoreRollbackInfo core = state as CoreRollbackInfo;
        //Should never be hit
        //if (core == null)
        //    return;

        _velocity = core.m_velocity;

        _characterController.enabled = false;
        transform.position = core.m_worldPosition;
        _characterController.enabled = true;
        _moveInput = core.m_moveInput;
        _dashTimer = core.m_remainingDashTime;

        //Adjust rotation to match input
        switch (_moveInput)
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
        //Jump
        if (core.m_jumping)
            Jump();
    }

    public virtual void Simulate(float delta)
    {
        SimulateMove(delta);
    }

    public BehaviourState CreateState()
    {
        return new CoreRollbackInfo(_currentHealth, _velocity, transform.position, (byte)_airCharges, _dashTimer, 0f, (byte)canMoveInt, (sbyte)_moveInput, _inputInfo.jump);
    }

    public void RefreshState(BehaviourState state)
    {
        CoreRollbackInfo core = state as CoreRollbackInfo;

        core.m_worldPosition = transform.position;
        core.m_velocity = _velocity;
        core.m_health = _currentHealth;
        core.m_remainingDashTime = _dashTimer;
    }
    /// <summary>
    /// Applies rollback by updating the players input
    /// </summary>
    /// <param name="time"></param>
    /// <param name="inputInfo"></param>
    [PunRPC]
    public void RPCRollback(float time, byte inputInfo)
    {
        Debug.LogError("Applying Rollback: Time: " + time);
        InputInfo newInput = InputInfo.FromByte(inputInfo);

        RollbackMaster.ApplyRollback(time, () =>
        {
            _moveInput = newInput.MoveInput;
            //Jump
            if (newInput.jump)
                Jump();
            //Dash
            if (newInput.dash)
                Dash();
        });
    }

    public void SimulateStart()
    {
        _simulationMode = true;
    }
    public void SimulateEnd()
    {
        _simulationMode = false;
    }
    #endregion
}
