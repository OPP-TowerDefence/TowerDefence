using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class FastEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y)
        {
            return new FastEnemy(x, y);
        }
    }
}
