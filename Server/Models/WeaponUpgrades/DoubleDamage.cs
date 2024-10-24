using System.Reflection.Metadata.Ecma335;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class DoubleDamage : WeaponDecorator
    {
        public DoubleDamage(IWeapon weapon) : base(weapon)
        {
        }
        public override int GetDamage()
        {
            return base.GetDamage()*2;
        }

        public override List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int damage, int numbEnemies)
        {
            return base.Shoot(tower, enemies, GetDamage());
        }
    }
}
