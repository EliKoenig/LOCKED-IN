using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    public PatrolState patrolState;

    public void Initialize()
    {
        patrolState = new PatrolState();
        ChangeState(patrolState);
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (activeState != null)
        {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if (activeState != null)
        {
            activeState.Exit();
        }
        activeState = newState;

        if (activeState != null)
        {
            //setup new state
            activeState.stateMachince = this;
            activeState.enemy = GetComponent<EnemyGun>();   
            activeState.Enter();
        }
    }
}
