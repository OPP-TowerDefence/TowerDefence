using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models.Enemies
{
    public abstract class Enemy : Unit
    {
        public Guid Id { get; set; }
        public abstract EnemyTypes Type { get; }
        public int Health { get; set; }
        public int RewardValue { get; set; } = 10;
        public int Speed { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }

        public Enemy(int x, int y) : base(x, y)
        {
            Id = Guid.NewGuid();
            X = x;
            Y = y;
        }

        public bool HasReachedDestination()
        {
            return X == TargetX && Y == TargetY;
        }

        public void MoveTowardsTarget()
        {
            if (X < TargetX)
                X += Math.Min(Speed, TargetX - X);
            if (Y < TargetY)
                Y += Math.Min(Speed, TargetY - Y);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public bool IsDead() => Health <= 0;
        public int DistanceTo(Tower tower) => Math.Abs(tower.X - X) + Math.Abs(tower.Y - Y);

        public void IncreaseHealth(int amount)
        {
            Health += amount;
        }

        public void IncreaseSpeed(int amount)
        {
            Speed += amount;
        }
    }
}
