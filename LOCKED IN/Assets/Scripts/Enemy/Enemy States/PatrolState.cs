using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public float waitTimer;
    

    public void Awake()
    {
       
    }
    public override void Enter()
    {

    }
    public override void Perform()
    {
        PatrolCycle();
    }
    public override void Exit()
    {

    }

    public void PatrolCycle()
    {
        if (enemy.Agent.remainingDistance < 1.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 3)
            {
                if (waypointIndex < enemy.waypoints.Count - 1)
                    waypointIndex++;
                else
                    waypointIndex = 0;
                enemy.Agent.SetDestination(enemy.waypoints[waypointIndex].position);
            }
        }

    }
} 

