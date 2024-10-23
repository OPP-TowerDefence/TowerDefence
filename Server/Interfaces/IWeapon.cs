using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Interfaces
{
    public interface IWeapon
    {
        public List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int numb = 1);
        public int GetDamage();
    }
}
