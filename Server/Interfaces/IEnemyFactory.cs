using TowerDefense.Models;

namespace TowerDefense.Interfaces
{
    public interface IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y, List<PathPoint> path);
    }
}
