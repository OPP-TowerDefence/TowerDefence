using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;
namespace TowerDefense.Models.Enemies
{
    public class FastEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y,List<PathPoint> path)
        {
            var enemy = new FastEnemy(x, y, path);
            enemy.SetInitialStrategy(new SpeedPrioritizationStrategy());
            return enemy;
        }
    }
}
