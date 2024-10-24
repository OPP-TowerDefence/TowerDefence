namespace TowerDefense.Models
{
    public class PathPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }  // Include the type of tile

        public PathPoint(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }
}
