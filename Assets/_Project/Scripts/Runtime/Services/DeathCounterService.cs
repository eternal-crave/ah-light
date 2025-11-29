namespace Runtime.Services
{
    public class DeathCounterService : IDeathCounterService
    {
        #region PRIVATE_FIELDS

        private int _deathCount;

        #endregion

        #region PROPERTIES

        public int DeathCount => _deathCount;

        #endregion

        #region PUBLIC_METHODS

        public void IncrementDeathCount()
        {
            _deathCount++;
        }

        public void ResetDeathCount()
        {
            _deathCount = 0;
        }

        #endregion
    }
}

