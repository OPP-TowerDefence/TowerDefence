namespace TowerDefense.Models
{
    public class Map
    {
        public int MapWidth { get; private set; } = 10;
        public int MapHeight { get; private set; } = 10;

        public Map(int mapWidth, int mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
        }
    }
}
