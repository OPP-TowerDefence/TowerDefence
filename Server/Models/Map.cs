namespace TowerDefense.Models
{
    public class Map
    {
        public int Width { get; private set; } = 10;
        public int Height { get; private set; } = 10;

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
