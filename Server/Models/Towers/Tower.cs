using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Models.Towers;
public abstract class Tower : Unit
{
    public int Cost { get; set; }
    public IWeapon Weapon { get; set; }
    public int TicksToShoot { get; set; }
    protected int TicksSinceLastShot { get; set; }
    public Armor Armor { get; set; }
    public abstract TowerTypes Type { get; }
    public abstract TowerCategories Category { get; }
    public abstract string BulletFileName { get; }
    public List<TowerUpgrades> AppliedUpgrades { get; set; }

    public Tower(int x, int y) : base(x, y)
    {
        TicksSinceLastShot = 0;
        AppliedUpgrades = new List<TowerUpgrades>();
    }

    public List<Bullet> Shoot(List<Enemy> enemies)
    {
        TicksSinceLastShot++;

        if (TicksSinceLastShot < TicksToShoot)
            return new List<Bullet>();

        TicksSinceLastShot = 0;
        return Weapon.Shoot(this, enemies, Weapon.GetDamage());
    }
}
