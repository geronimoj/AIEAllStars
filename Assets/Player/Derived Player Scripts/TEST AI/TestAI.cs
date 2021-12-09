using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI : Player
{
    public bool IsAI;


    public float AttackRange;


    // Update is called once per frame
    void Update()
    {
        if(DistanceFromPlayer() < AttackRange)
        {
            FacePlayer();
            Attack();
        }
   

        int MoveRange = Random.Range(-1, 2);
        Move(MoveRange);
    }

    float DistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, GameManager.s_p1Char.transform.position); 
    }

    bool PlayerIsOnLeft()
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

    void FacePlayer()
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

    void FaceRight()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
    }

    void FaceLeft()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }
}
