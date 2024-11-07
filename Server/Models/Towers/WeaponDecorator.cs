using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Models.Towers
{
    public class WeaponDecorator(IWeapon weapon) : IWeapon
    {
        private IWeapon _weapon = weapon;

        public virtual int GetRange()
        {
            return _weapon.GetRange();
        }
        
        public virtual int GetDamage()
        {
            return _weapon.GetDamage();
        }

        public virtual List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int damage, int numbEnemies = 1)
        {
            return _weapon.Shoot(tower, enemies, damage, numbEnemies);
        }

        public virtual void IncreaseDamage(int amount)
        {
            _weapon.IncreaseDamage(amount);
        }

        public virtual void IncreaseRange(int amount)
        {
            _weapon.IncreaseRange(amount);
        }

        public virtual void IncreaseSpeed(int amount)
        {
            _weapon.IncreaseSpeed(amount);
        }
    }
}
