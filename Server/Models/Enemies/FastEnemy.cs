namespace TowerDefense.Models.Enemies
{
    public class FastEnemy : Enemy
    {
        public FastEnemy(int x, int y) : base(x, y)
        {
            Health = 20;
            Speed = 3;
        }
    }
}
