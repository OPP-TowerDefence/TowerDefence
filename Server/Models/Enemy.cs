namespace TowerDefense.Models
{
    public class Enemy : Unit
    {
        public int Health { get; set; }
        public int Speed { get; set; }

        public Enemy(int x, int y) : base(x, y)
        {
            X = x;
            Y = y;
        }

    }
}
