namespace TowerDefense.Models.Enemies
{
    public abstract class Enemy : Unit
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public int TargetX { get; set; } = 10;
        public int TargetY { get; set; } = 10;

        public void MoveTowardsTarget()
        {
            if (X < TargetX)
                X += Math.Min(Speed, TargetX - X);
            if (Y < TargetY)
                Y += Math.Min(Speed, TargetY - Y);
        }

        public bool HasReachedDestination()
        {
            return X == TargetX && Y == TargetY;
        }
    }
}
