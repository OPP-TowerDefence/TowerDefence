using TowerDefense.Enums;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class Bullet(int x, int y, Guid enemyId, int damage, BulletFlyweight flyweight)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public Guid EnemyId { get; set; } = enemyId;
        public int Damage { get; set; } = damage;
        public BulletFlyweight Flyweight { get; set; } = flyweight;

        public void Move(int targetX, int targetY)
        {
            int deltaX = targetX - X;
            int deltaY = targetY - Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance == 0)
            {
                return;
            }

            double moveX = (deltaX / distance) * Flyweight.Speed;
            double moveY = (deltaY / distance) * Flyweight.Speed;

            if (Math.Abs(moveX) >= Math.Abs(deltaX))
            {
                X = targetX;
            }
            else
            {
                X += (int)moveX;
            }

            if (Math.Abs(moveY) >= Math.Abs(deltaY))
            {
                Y = targetY;
            }
            else
            {
                Y += (int)moveY;
            }
        } 
    }
}
