using System.Runtime.CompilerServices;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Models.Towers
{
    public class Weapon : IWeapon
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Speed { get; set; }
        public int Range { get; set; }

        public Weapon(string name, int damage, int range, int speed)
        {
            Name = name;
            Damage = damage;
            Range = range;
            Speed = speed;
        }

        public List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int numb = 1)
        {
            var bullets = new List<Bullet>();
            if(enemies == null || enemies.Count == 0)
            {
                return bullets;
            }

            var nearestEnemy = CalculateNearestEnemies(tower, enemies, numb);
            foreach (var enemy in nearestEnemy)
            {
                bullets.Add(new Bullet(tower.X, tower.Y, enemy.Id, GetDamage(), Speed));
            }

            return bullets;
        }

        private List<Enemy> CalculateNearestEnemies(Tower tower, List<Enemy> enemies, int numb)
        {
            if (tower == null || enemies == null || enemies.Count == 0 || numb <= 0)
            {
                return new List<Enemy>();
            }

            return enemies.OrderBy(enemy => enemy.DistanceTo(tower)).Take(numb).ToList();
        }

        public int GetDamage()
        {
            return Damage;
        }
    }
}
