namespace TowerDefense.Models.Enemies
{
    public class FastEnemy : Enemy
    {
        public FastEnemy(int x, int y,List<(int X, int Y)> path) : base(x, y, path)
        {
            Health = 50;
            Speed = 3;
        }
    }
}
