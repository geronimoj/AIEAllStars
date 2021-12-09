using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI : Player
{
    public bool IsAI;

    public TimerData JumpData;
    public TimerData MoveData;

    protected int MoveInput;
    public float AttackRange;

    [System.Serializable]
    public class TimerData
    {
        public float MinTime;
        public float MaxTime;
        protected float CurrentTime;

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
            AIAttack();

            if (CallTimer(JumpData))
            {
                Jump();
            }

            if (CallTimer(MoveData))
            {
                ChooseMoveDirection();
            }

            Move(MoveInput);
        }
    }

    protected void AIAttack()
    {
        if (DistanceFromPlayer() < AttackRange)
        {
            FacePlayer();
            Attack();
        }
    }

    protected void ChooseMoveDirection()
    {
        MoveInput = Random.Range(-1, 2);
    }

    public bool CallTimer(TimerData Timer)
    {
        if (Timer.CallTimer())
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
