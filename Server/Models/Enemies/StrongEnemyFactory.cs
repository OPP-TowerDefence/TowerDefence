using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y)
        {
            return new StrongEnemy(x, y);
        }
    }
}
