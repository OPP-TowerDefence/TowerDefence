using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Models;

namespace TowerDefense.Utils
{
    public class LevelProgressionFacade(MainObject mainObject, List<Enemy> enemies, List<Tower> towers)
    {
        private const int HealthIncreasePerLevel = 20;
        private const int EnemyHealthIncrease = 10;
        private const int EnemySpeedIncrease = 1;
        private const int TowerDamageIncrease = 2;
        private const int TowerRangeIncrease = 1;
        private const int TowerSpeedIncrease = 2;

        private MainObject _mainObject = mainObject;
        private List<Enemy> _enemies = enemies;
        private List<Tower> _towers = towers;
        private int _currentLevel = 1;

        public void IncreaseLevel()
        {
            _currentLevel++;

            foreach (var enemy in _enemies)
            {
                int levelMultiplier = _currentLevel - 1;
                enemy.IncreaseHealth(EnemyHealthIncrease * levelMultiplier);
                if (_currentLevel >= 3)
                {
                    enemy.IncreaseSpeed(EnemySpeedIncrease);
                }
            }

            foreach (var tower in _towers)
            {
                tower.Weapon.IncreaseDamage(TowerDamageIncrease);
                tower.Weapon.IncreaseRange(TowerRangeIncrease);
                tower.Weapon.IncreaseSpeed(TowerSpeedIncrease);
            }

            _mainObject.IncreaseHealth(HealthIncreasePerLevel);
        }

        public void ApplyBuffToNewEnemy(Enemy enemy)
        {
            if (_currentLevel > 1)
            {
                int levelMultiplier = _currentLevel - 1;
                enemy.IncreaseHealth(EnemyHealthIncrease * levelMultiplier);

                if (_currentLevel >= 3)
                {
                    enemy.IncreaseSpeed(EnemySpeedIncrease);
                }
            }
        }

        public void ApplyBuffToNewTower(Tower tower)
        {
            if (_currentLevel > 1)
            {
                int levelMultiplier = _currentLevel - 1;
                tower.Weapon.IncreaseDamage(TowerDamageIncrease * levelMultiplier);
                tower.Weapon.IncreaseRange(TowerRangeIncrease * levelMultiplier);
                tower.Weapon.IncreaseSpeed(TowerSpeedIncrease * levelMultiplier);
            }
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }
    }
}

