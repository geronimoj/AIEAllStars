using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI : Player
{
    [Header("AI Customisation")]

    [Space]
    public bool IsAI = true;
    protected int MoveInput = 0;

    [Space]
    public TimerData MoveData;
    public TimerData JumpData;
    public TimerData DashData;
    public TimerData SkillData;

    [Space]
    public float AttackRange = 1;

    [System.Serializable]
    public class TimerData
    {
        public float MinTime = 3;
        public float MaxTime = 5;
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

    // Update is called once per frame
    void Update()
    {
        if (IsAI)
        {
            Move(MoveInput);

            if (DistanceFromPlayer() < AttackRange)
            {
                Debug.Log("Attack");
                AIAttack();
            }

            if (CallTimer(JumpData))
            {
                Debug.Log("Jump");
                Jump();
            }

            if (CallTimer(DashData))
            {
                Debug.Log("Dash");
                Dash();
            }

            if (CallTimer(SkillData))
            {
                if (AISkillCanBeUsed())
                {
                    Debug.Log("Skill");
                    Skill();
                }
            }

            if (CallTimer(MoveData))
            {
                ChooseMoveDirection();
            }
        }
    }

    protected virtual bool AISkillCanBeUsed()
    {
        //Need to override in each different character. This is the trigger for each different character to use the skill. It also has to line up with the timer hitting 0.
        //some characters wont need triggers and therefore will just have return true
        //example trigger

        int trigger = Random.Range(0, 2);

        if(trigger == 0)
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
        FacePlayer();
        Attack();
    }

    protected void ChooseMoveDirection()
    {
        MoveInput = Random.Range(-1, 2);

        switch(MoveInput)
        {
            case -1:
                FaceLeft();
                break;

            case 0:
                FacePlayer();
                break;

            case 1:
                FaceRight();
                break;
        }
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

    protected float DistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, GameManager.s_p1Char.transform.position); 
    }

    protected bool PlayerIsOnLeft()
    {
        if(GameManager.s_p1Char.transform.position.x > transform.position.x)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected void FacePlayer()
    {
        if(PlayerIsOnLeft())
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
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
    }

    protected void FaceLeft()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }
}
