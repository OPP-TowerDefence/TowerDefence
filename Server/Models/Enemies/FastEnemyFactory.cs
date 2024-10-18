using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class FastEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y,List<(int X, int Y)> path)
        {
            return new FastEnemy(x, y, path);
        }
    }
}
