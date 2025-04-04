using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class VStateMachine : MonoBehaviour
{
    public VBaseState activeState;

    public void Initialize()
    {

        ChangeState(new VPatrolState());
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

    public void ChangeState(VBaseState newState)
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
            activeState.enemy = GetComponent<VEnemyGun>();
            activeState.Enter();
        }
    }
}