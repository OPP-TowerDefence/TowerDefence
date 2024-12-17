using TowerDefense.Interfaces;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.Mementos
{
    public class GameStateMemento
    {
        private readonly int _currentLevel;
        private readonly List<Tower> _towers;
        private readonly List<IEnemyComponent> _enemies;
        private readonly int _resources;
        private readonly Map _map;
        private readonly int _mainObjectHealth;
        private readonly string _mainObjectState;

        private readonly Guid _accessToken;

        public GameStateMemento(Guid accessToken, int currentLevel, List<Tower> towers, List<IEnemyComponent> enemies, int resources, Map map, int mainObjectHealth, string mainObjectState)
        {
            _accessToken = accessToken;
            _currentLevel = currentLevel;
            _towers = towers;
            _enemies = enemies;
            _resources = resources;
            _map = map;
            _mainObjectHealth = mainObjectHealth;
            _mainObjectState = mainObjectState;
        }

        public (int CurrentLevel, List<Tower> Towers, List<IEnemyComponent> Enemies, int Resources, Map GameMap, int MainObjectHealth, string MainObjectState) GetState(Guid requesterToken)
        {
            if (requesterToken != _accessToken)
            {
                throw new InvalidOperationException("Access denied: Invalid access token.");
            }

            return (_currentLevel, _towers, _enemies, _resources, _map, _mainObjectHealth, _mainObjectState);
        }
    }
}
