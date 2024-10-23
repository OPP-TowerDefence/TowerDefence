using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class Bullet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Guid EnemyId { get; set; }
        public int Damage { get; set; }
        public int Speed { get; set; }

        public Bullet(int x, int y, Guid enemyId, int damage, int speed)
        {
            X = x;
            Y = y;
            EnemyId = enemyId;
            Damage = damage;
            Speed = speed;
        }

        public void Move(int targetX, int targetY)
        {
            Console.WriteLine($"Before move: X = {X}, Y = {Y}, TargetX = {targetX}, TargetY = {targetY}");

            int deltaX = targetX - X;
            int deltaY = targetY - Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance == 0)
            {
                return;
            }

            double moveX = (deltaX / distance) * Speed;
            double moveY = (deltaY / distance) * Speed;

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

            Console.WriteLine($"After move: X = {X}, Y = {Y}");
        }

    }
}
