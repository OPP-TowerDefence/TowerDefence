namespace TowerDefense.Utils
{
    public class LevelManager(LevelProgressionFacade levelFacade)
    {
        private int _enemiesSpawned = 0;
        private int _baseEnemiesPerLevel = 10;
        private int _currentLevel = 1;

        private readonly LevelProgressionFacade _levelFacade = levelFacade;

        public event Action<int>? OnLevelChanged;

        public void OnEnemySpawned()
        {
            _enemiesSpawned++;

            int enemiesRequiredForNextLevel = _baseEnemiesPerLevel * _currentLevel * (_currentLevel + 1) / 2;

            if (_enemiesSpawned >= enemiesRequiredForNextLevel)
            {
                _levelFacade.IncreaseLevel();
                _currentLevel = _levelFacade.GetCurrentLevel();

                OnLevelChanged?.Invoke(_currentLevel);
            }
        }
    }
}
