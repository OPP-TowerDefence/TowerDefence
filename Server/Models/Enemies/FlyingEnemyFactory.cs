using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy(int x, int y)
        {
            return new FlyingEnemy(x, y);
        }
    }
}
