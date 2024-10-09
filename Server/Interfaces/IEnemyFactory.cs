using TowerDefense.Models.Enemies;

namespace TowerDefense.Interfaces
{
    public interface IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y);
    }
}
