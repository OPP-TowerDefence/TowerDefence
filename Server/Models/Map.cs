using TowerDefense.Models.Towers;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Models
{
    public class Map
    {
        public int Width { get; private set; } = 10;
        public int Height { get; private set; } = 10;
        public List<Tower> Towers { get; set; } = new List<Tower>();
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();
        public List<Bullet> Bullets { get; set; } = new List<Bullet>();

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
