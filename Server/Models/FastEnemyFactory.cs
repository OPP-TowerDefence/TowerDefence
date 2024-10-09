namespace TowerDefense.Models
{
    public class FastEnemyFactory : EnemyFactory
    {
        public override Enemy CreateEnemy()
        {
            return new FastEnemy { X = 0, Y = 0 };
        }
    }
}
