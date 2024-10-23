using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class Burst : WeaponDecorator
    {
        public Burst(IWeapon weapon) : base(weapon)
        {
        }

        public override List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int numb)
        {
            return base.Shoot(tower, enemies, 2);
        }
    }
}
