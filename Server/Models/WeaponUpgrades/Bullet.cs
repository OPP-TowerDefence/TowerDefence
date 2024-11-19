using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.WeaponUpgrades
{
    public class Bullet(int x, int y, Guid enemyId, int damage, int speed, string fileName)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public Guid EnemyId { get; set; } = enemyId;
        public int Damage { get; set; } = damage;
        public int Speed { get; set; } = speed;
        public string FileName { get; set; } = fileName;
        public string Path = $"{_baseUrl}/Bullets/{fileName}";

        private static readonly string _baseUrl = "http://localhost:7041";

        public void Move(int targetX, int targetY)
        {
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
        }
    }
}
