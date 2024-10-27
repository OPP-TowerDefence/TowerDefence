using TowerDefense.Enums;

namespace TowerDefense.Models.Enemies
{
    public class FastEnemy : Enemy
    {
        public override EnemyTypes Type => EnemyTypes.Fast;
        public FastEnemy(int x, int y) : base(x, y)
        {
            Health = 20;
            Speed = 3;
        }
    }
}
