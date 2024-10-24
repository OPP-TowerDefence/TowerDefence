namespace TowerDefense.Utils
{
    public class LevelManager
    {
        private int enemiesSpawned = 0;
        private int baseEnemiesPerLevel = 10;
        private int currentLevel = 1;
        private LevelProgressionFacade _levelFacade;

        public LevelManager(LevelProgressionFacade levelFacade)
        {
            _levelFacade = levelFacade;
        }

        public event Action<int>? OnLevelChanged;

        public void OnEnemySpawned()
        {
            enemiesSpawned++;

            int enemiesRequiredForNextLevel = baseEnemiesPerLevel * currentLevel * (currentLevel + 1) / 2;

            if (enemiesSpawned >= enemiesRequiredForNextLevel)
            {
                _levelFacade.IncreaseLevel();
                currentLevel = _levelFacade.GetCurrentLevel();

                OnLevelChanged?.Invoke(currentLevel);
            }
        }
    }
}
