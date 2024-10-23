namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemy : Enemy
    {
        public FlyingEnemy(int x, int y) : base(x, y )
        {
            Health = 25;
            Speed = 2;
        }
    }
}
