using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class FastEnemyFactory : IEnemyFactory
    {
        private readonly FastEnemy _prototypeEnemy;
        
        public FastEnemyFactory()
        {
            _prototypeEnemy = new FastEnemy(0, 0);
            _prototypeEnemy.SetInitialStrategy(new SpeedPrioritizationStrategy());
        }

        public Enemy CreateEnemy(int x, int y)
        {
            var enemy = _prototypeEnemy.ShallowClone();
            enemy.X = x;
            enemy.Y = y;
            return enemy;
        }
    }
}
