using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class DoubleBullet(IWeapon weapon) : WeaponDecorator(weapon)
    {
        public override List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int damage, int numbEnemies)
        {
            var bullets = base.Shoot(tower, enemies, damage, numbEnemies);
            var resultBullets = new List<Bullet>();

            foreach (var firstBullet in bullets)
            {
                var enemy = enemies.FirstOrDefault(e => e.Id == firstBullet.EnemyId);

                if (enemy == null)
                {
                    resultBullets.Add(firstBullet);
                    continue;
                }

                Bullet secondBullet = CalculateSecondBullet(firstBullet, enemy);

                resultBullets.Add(firstBullet);
                resultBullets.Add(secondBullet);
            }
            return resultBullets;
        }


        private Bullet CalculateSecondBullet(Bullet firstBullet, Enemy enemy)
        {
            int deltaX = enemy.X - firstBullet.X;
            int deltaY = enemy.Y - firstBullet.Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance == 0)
            {
                return new Bullet(firstBullet.X, firstBullet.Y, firstBullet.EnemyId, firstBullet.Damage, firstBullet.Speed, firstBullet.FileName);
            }

            double normalizedX = deltaX / distance;
            double normalizedY = deltaY / distance;

            int secondBulletX = firstBullet.X + (int)Math.Round(normalizedX);
            int secondBulletY = firstBullet.Y + (int)Math.Round(normalizedY);

            return new Bullet(secondBulletX, secondBulletY, firstBullet.EnemyId, firstBullet.Damage, firstBullet.Speed, firstBullet.FileName);
        }

    }
}
