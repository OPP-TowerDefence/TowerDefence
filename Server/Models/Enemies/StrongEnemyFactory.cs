using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        private readonly StrongEnemy _prototypeEnemy;

        public StrongEnemyFactory()
        {
            _prototypeEnemy = new StrongEnemy(0, 0);
            _prototypeEnemy.SetInitialStrategy(new SurvivalStrategy());
        }

        public Enemy CreateEnemy(int x, int y)
        {
            var enemy = _prototypeEnemy.Clone();
            enemy.X = x;
            enemy.Y = y;
            return enemy;
        }
    }
}
