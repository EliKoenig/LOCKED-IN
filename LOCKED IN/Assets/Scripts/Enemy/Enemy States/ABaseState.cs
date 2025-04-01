public abstract class ABaseState
{

    public AEnemyGun enemy;
    public AStateMachine stateMachine;

    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}