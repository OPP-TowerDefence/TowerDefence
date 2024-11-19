using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Models.Towers
{
    public class Weapon(string name, int damage, int range, int speed) : IWeapon
    {
        public string Name { get; set; } = name;
        public int Damage { get; set; } = damage;
        public int Range { get; set; } = range;
        public int Speed { get; set; } = speed;

        public virtual int GetDamage()
        {
            return Damage;
        }
        
        public virtual int GetRange()
        {
            return Range;
        }

        public void IncreaseDamage(int amount)
        {
            Damage += amount;
        }

        public void IncreaseRange(int amount)
        {
            Range += amount;
        }

        public void IncreaseSpeed(int amount)
        {
            Speed += amount;
        }

        public List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int damage, int numbEnemies = 1)
        {
            var bullets = new List<Bullet>();

            if (enemies == null || enemies.Count == 0)
            {
                return bullets;
            }

            var nearestEnemy = CalculateNearestEnemies(tower, enemies, numbEnemies);

            foreach (var enemy in nearestEnemy)
            {
                bullets.Add(new Bullet(tower.X, tower.Y, enemy.Id, damage, Speed, tower.BulletFileName));
            }

            return bullets;
        }

        private List<Enemy> CalculateNearestEnemies(Tower tower, List<Enemy> enemies, int numb)
        {
            if (tower == null || enemies == null || enemies.Count == 0 || numb <= 0)
            {
                return [];
            }

            return enemies.Where(enemy => enemy.DistanceTo(tower) <= Range)
                  .OrderBy(enemy => enemy.DistanceTo(tower))
                  .Take(numb)
                  .ToList();
        }
    }
}
