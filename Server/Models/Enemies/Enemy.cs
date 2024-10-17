using TowerDefense.Utils;

namespace TowerDefense.Models.Enemies
{
    public abstract class Enemy(int x, int y) : Unit(x, y)
    {
        public int Health { get; set; }
        public int RewardValue { get; set; } = 10;
        public int Speed { get; set; }
        public int TargetX { get; set; } = 10;
        public int TargetY { get; set; } = 10;

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

        public void TakeDamage(int damage, ResourceManager resourceManager)
        {
            Health -= damage;

            if (Health <= 0)
            {
                resourceManager.OnEnemyDied(this);
            }
        }
    }
}
