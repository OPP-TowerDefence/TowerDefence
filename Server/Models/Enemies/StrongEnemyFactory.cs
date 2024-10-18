using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y, List<(int X, int Y)> path)
        {
            return new StrongEnemy(x, y, path);
        }
    }
}
