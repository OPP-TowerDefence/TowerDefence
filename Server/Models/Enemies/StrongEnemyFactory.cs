using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y, List<PathPoint> path)
        {
            var enemy = new StrongEnemy(x, y, path);
            enemy.SetInitialStrategy(new SurvivalPrioritizationStrategy());
            return enemy;
        }
    }
}
