using TowerDefense.Enums;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemy : Enemy
    {
        public override EnemyTypes Type => EnemyTypes.Flying;
        public FlyingEnemy(int x, int y) : base(x, y )
        {
            Health = 25;
            Speed = 2;
        }
    }
}
