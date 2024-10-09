namespace TowerDefense.Models
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        public  Enemy CreateEnemy()
        {
            return new StrongEnemy { X = 0, Y = 0 };
        }
    }
}
