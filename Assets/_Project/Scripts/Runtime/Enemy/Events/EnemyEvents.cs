namespace Runtime.Enemy.Events
{
    public class PlayerKilledEvent
    {
        public EnemyControllerBase Killer { get; }

        public PlayerKilledEvent(EnemyControllerBase killer)
        {
            Killer = killer;
        }
    }

    public class EnemyDisappearedEvent
    {
        public EnemyControllerBase Enemy { get; }
        public DisappearReason Reason { get; }

        public EnemyDisappearedEvent(EnemyControllerBase enemy, DisappearReason reason)
        {
            Enemy = enemy;
            Reason = reason;
        }
    }

    public enum DisappearReason
    {
        TorchHeld,
        PatrolComplete,
        Destroyed
    }
}

