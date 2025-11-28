namespace Runtime.Enemy.States
{
    public interface IEnemyState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}

