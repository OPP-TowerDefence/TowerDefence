using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy()
        {
            return new StrongEnemy { X = 0, Y = 0 };
        }
    }
}
