using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y)
        {
            var enemy = new FlyingEnemy(x, y);
            enemy.SetInitialStrategy(new SpeedPrioritizationStrategy());
            return enemy;
        }
    }
}
