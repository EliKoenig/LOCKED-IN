using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class AStateMachine : MonoBehaviour
{
    public ABaseState activeState;

    public void Initialize()
    {

        ChangeState(new APatrolState());
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

    public void ChangeState(ABaseState newState)
    {
        if (activeState != null)
        {
            activeState.Exit();
        }
        activeState = newState;

        if (activeState != null)
        {
            //setup new state
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<AEnemyGun>();
            activeState.Enter();
        }
    }
}
