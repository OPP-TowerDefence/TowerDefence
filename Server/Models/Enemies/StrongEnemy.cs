namespace TowerDefense.Models.Enemies
{
    public class StrongEnemy : Enemy
    {
        public StrongEnemy(int x, int y,List<PathPoint> path) : base(x, y,path)
        {
            Health = 150;
            Speed = 1;
        }
    }
}
