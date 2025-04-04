public abstract class VBaseState
{

    public VEnemyGun enemy;
    public VStateMachine stateMachine;

    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}