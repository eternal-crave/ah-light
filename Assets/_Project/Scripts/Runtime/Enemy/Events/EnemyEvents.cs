namespace Runtime.Enemy.Events
{
    public class PlayerKilledEvent
    {
        #region PUBLIC_PROPERTIES

        public EnemyControllerBase Killer { get; }

        #endregion

        #region CONSTRUCTORS

        public PlayerKilledEvent(EnemyControllerBase killer)
        {
            Killer = killer;
        }

        #endregion
    }

    public class EnemyDisappearedEvent
    {
        #region PUBLIC_PROPERTIES

        public EnemyControllerBase Enemy { get; }
        public DisappearReason Reason { get; }

        #endregion

        #region CONSTRUCTORS

        public EnemyDisappearedEvent(EnemyControllerBase enemy, DisappearReason reason)
        {
            Enemy = enemy;
            Reason = reason;
        }

        #endregion
    }

    #region ENUMS

    public enum DisappearReason
    {
        TorchHeld,
        PatrolComplete,
        Destroyed
    }

    #endregion
}

