namespace Runtime.Services
{
    public interface IDeathCounterService
    {
        #region PROPERTIES

        int DeathCount { get; }

        #endregion

        #region PUBLIC_METHODS

        void IncrementDeathCount();
        void ResetDeathCount();

        #endregion
    }
}

