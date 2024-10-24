using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Models;

namespace TowerDefense.Utils
{
    public class LevelProgressionFacade
    {
        private GameState _gameState;
        private List<Enemy> _enemies;
        private List<Tower> _towers;
        private int _currentLevel;

        public LevelProgressionFacade(GameState gameState, List<Enemy> enemies, List<Tower> towers)
        {
            _gameState = gameState;
            _enemies = enemies;
            _towers = towers;
            _currentLevel = 1;
        }

        public void IncreaseLevel()
        {
            _currentLevel++;

            foreach (var enemy in _enemies)
            {
                enemy.IncreaseHealth(10 * _currentLevel);
                enemy.IncreaseSpeed(1);
                Console.WriteLine($"Enemy health increased to {enemy.Health}, speed increased to {enemy.Speed}");
            }

            foreach (var tower in _towers)
            {         
                tower.Weapon.IncreaseDamage(2);
                tower.Weapon.IncreaseRange(1);
                tower.Weapon.IncreaseSpeed(2);
                Console.WriteLine($"Tower damage increased to {tower.Weapon.GetDamage()}");         
            }

            _gameState.IncreaseHealth(20);
            Console.WriteLine($"Game state health {_gameState.Health})");
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }
    }
}
