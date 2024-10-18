namespace TowerDefense.Models.Enemies
{
    public class StrongEnemy : Enemy
    {
        public StrongEnemy(int x, int y,List<(int X, int Y)> path) : base(x, y,path)
        {
            Health = 150;
            Speed = 1;
        }
    }
}
