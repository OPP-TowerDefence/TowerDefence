using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class DoubleBullet : WeaponDecorator
    {
        public DoubleBullet(IWeapon weapon) : base(weapon)
        {
        }

        public override List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int numb)
        {
            var firstBullets = base.Shoot(tower, enemies);
            var secondBullet = base.Shoot(tower, enemies);
            firstBullets.AddRange(secondBullet);

            return firstBullets;
        }
    }
}
