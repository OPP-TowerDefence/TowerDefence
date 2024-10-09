namespace TowerDefense.Models
{
    public class FlyingEnemyFactory : EnemyFactory
    {
        public override Enemy CreateEnemy()
        {
            return new FlyingEnemy {X = 0, Y = 0 };
        }
    }
}
