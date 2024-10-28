using TowerDefense.Enums;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemy : Enemy
    {
        public override EnemyTypes Type => EnemyTypes.Strong;

        public StrongEnemy(int x, int y) : base(x, y)
        {
            Health = 30;
            Speed = 1;
        }
    }
}
