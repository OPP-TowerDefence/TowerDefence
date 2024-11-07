using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemyFactory : IEnemyFactory
    {
        private readonly FlyingEnemy _prototypeEnemy;

        public FlyingEnemyFactory()
        {
            _prototypeEnemy = new FlyingEnemy(0, 0);
            _prototypeEnemy.SetInitialStrategy(new SpeedPrioritizationStrategy());
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
