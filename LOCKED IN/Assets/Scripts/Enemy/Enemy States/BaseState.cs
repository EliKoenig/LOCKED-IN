public abstract class BaseState
{

    public EnemyGun enemy;
    public StateMachine stateMachince;

    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}