namespace TowerDefense.Models
{
    public class StrongEnemyFactory : EnemyFactory
    {
        public override Enemy CreateEnemy()
        {
            return new StrongEnemy { X = 0, Y = 0 };
        }
    }
}
