using TowerDefense.Enums;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models
{
    public class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private TileType[,] tiles;

        public List<Tower> Towers { get; set; } = new List<Tower>();
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            InitializeTiles();
            PlaceObjectiveTile();
        }

        private void InitializeTiles()
        {
            tiles = new TileType[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tiles[x, y] = TileType.Normal;
                }
            }
        }

        private void PlaceObjectiveTile()
        {
            int objectiveX = Width - 1; // Rightmost column
            int objectiveY = Height - 1; // Bottom row

            // Place the objective tile at the rightmost bottom position
            tiles[objectiveX, objectiveY] = TileType.Objective;
        }

        public void SetTileType(int x, int y, TileType newTileType)
        {
            if (IsValidCoordinate(x, y))
            {
                tiles[x, y] = newTileType;
            }
        }

        public TileType GetTileType(int x, int y)
        {
            if (IsValidCoordinate(x, y))
            {
                return tiles[x, y];
            }
            return TileType.Normal;
        }

        private bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool IsTileDefended(int x, int y)
        {
            return Towers.Exists(tower => tower.IsInRange(x, y));
        }

        public (int X, int Y) GetObjectiveTile()
        {
            return (Width - 1, Height - 1);
        }
        public bool IsOccupied(int x, int y)
        {
            if (!IsValidCoordinate(x, y))
            {
                return false;
            }

            if (Towers.Any(tower => tower.X == x && tower.Y == y))
            {
                return true;
            }

            if (Enemies.Any(enemy => enemy.X == x && enemy.Y == y))
            {
                return true;
            }

            return false;
        }
    }
}
