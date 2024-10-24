using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y,List<PathPoint> path)
        {
            var enemy = new FlyingEnemy(x, y, path);
            enemy.SetInitialStrategy(new GroupProtectionStrategy());
            return enemy;
        }
    }
}
