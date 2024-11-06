using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y)
        {
            var enemy = new StrongEnemy(x, y);
            enemy.SetInitialStrategy(new SurvivalStrategy());
            return enemy;
        }
    }
}
