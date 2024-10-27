using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Models;

namespace TowerDefense.Utils
{
    public class LevelProgressionFacade
    {
        private MainObject _mainObject;
        private List<Enemy> _enemies;
        private List<Tower> _towers;
        private int _currentLevel;

        public LevelProgressionFacade(MainObject mainObject, List<Enemy> enemies, List<Tower> towers)
        {
            _mainObject = mainObject;
            _enemies = enemies;
            _towers = towers;
            _currentLevel = 1;
        }

        public void IncreaseLevel()
        {
            _currentLevel++;

            foreach (var enemy in _enemies)
            {
                int levelMultiplier = _currentLevel - 1;
                enemy.IncreaseHealth(10 * levelMultiplier);
                if (_currentLevel >= 3)
                {
                    enemy.IncreaseSpeed(1);
                }
                Console.WriteLine($"Enemy health increased : {enemy.Health} Enemy speed increased : {enemy.Speed}");
            }

            foreach (var tower in _towers)
            {
                tower.Weapon.IncreaseDamage(2);
                tower.Weapon.IncreaseRange(1);
                tower.Weapon.IncreaseSpeed(2);
                Console.WriteLine($"Tower damage increased : {tower.Weapon.GetDamage()}");
            }

            _mainObject.IncreaseHealth(20);
            Console.WriteLine($"Health is {_mainObject.Health}");
        }

        public void ApplyBuffToNewEnemy(Enemy enemy)
        {
            if (_currentLevel > 1)
            {
                int levelMultiplier = _currentLevel - 1;
                enemy.IncreaseHealth(10 * levelMultiplier);

                if (_currentLevel >= 3)
                {
                    enemy.IncreaseSpeed(1);
                }
            }
        }

        public void ApplyBuffToNewTower(Tower tower)
        {
            if (_currentLevel > 1)
            {
                int levelMultiplier = _currentLevel - 1;
                tower.Weapon.IncreaseDamage(2 * levelMultiplier);
                tower.Weapon.IncreaseRange(1 * levelMultiplier);
                tower.Weapon.IncreaseSpeed(2 * levelMultiplier);
            }
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }
    }
}

