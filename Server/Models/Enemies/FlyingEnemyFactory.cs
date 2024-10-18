using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y,List<(int X, int Y)> path)
        {
            return new FlyingEnemy(x, y, path);
        }
    }
}
