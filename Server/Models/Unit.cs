namespace TowerDefense.Models
{
    public abstract class Unit(int x, int y)
    {
        public int X { get; set; } = x;

        public int Y { get; set; } = y;
    }
}
