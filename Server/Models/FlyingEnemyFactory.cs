namespace TowerDefense.Models
{
    public class FlyingEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy()
        {
            return new FlyingEnemy {X = 0, Y = 0 };
        }
    }
}
