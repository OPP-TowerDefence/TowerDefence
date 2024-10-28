using TowerDefense.Models.Towers;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Models
{
    public class Map(int width, int height)
    {
        public int Width { get; private set; } = width;
        public int Height { get; private set; } = height;
        public List<Tower> Towers { get; set; } = [];
        public List<Enemy> Enemies { get; set; } = [];
        public List<Bullet> Bullets { get; set; } = [];
        public MainObject MainObject { get; private set; } = new MainObject(10, 10);

        public bool IsOccupied(int x, int y)
        {
            return Towers.Any(t => t.X == x && t.Y == y);
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
