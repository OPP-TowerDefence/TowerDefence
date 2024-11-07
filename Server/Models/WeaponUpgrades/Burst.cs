using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class Burst(IWeapon weapon) : WeaponDecorator(weapon)
    {
        public override List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int damage, int numbEnemies)
        {
            numbEnemies = Math.Max(numbEnemies, 2);
            return base.Shoot(tower, enemies, damage, numbEnemies);
        }
    }
}
