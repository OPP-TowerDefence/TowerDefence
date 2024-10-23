namespace TowerDefense.Models.Enemies
{
    public class StrongEnemy : Enemy
    {
        public StrongEnemy(int x, int y) : base(x, y)
        {
            Health = 30;
            Speed = 1;
        }
    }
}
