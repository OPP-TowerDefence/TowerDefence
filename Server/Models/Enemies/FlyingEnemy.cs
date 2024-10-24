namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemy : Enemy
    {
        public FlyingEnemy(int x, int y,List<PathPoint> path) : base(x, y, path)
        {
            Health = 75;
            Speed = 2;
        }
    }
}
