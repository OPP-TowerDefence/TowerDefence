using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Models.Towers
{
    public class WeaponDecorator : IWeapon
    {
        private IWeapon _weapon;

        public WeaponDecorator(IWeapon weapon)
        {
            _weapon = weapon;
        }

        public virtual int GetDamage()
        {
            return _weapon.GetDamage();
        }

        public virtual List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int damage, int numbEnemies = 1)
        {
            return _weapon.Shoot(tower, enemies, damage, numbEnemies);
        }

    }
}
