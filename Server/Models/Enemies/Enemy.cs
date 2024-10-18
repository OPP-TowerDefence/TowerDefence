namespace TowerDefense.Models.Enemies
{
    public abstract class Enemy : Unit
{
    public int Health { get; set; }
    public int Speed { get; set; }
    private Queue<(int X, int Y)> Path { get; set; } // Store the path

    private bool IsAtFinalDestination { get; set; } = false; // Track if the enemy has reached the final tile

    public Enemy(int x, int y, List<(int X, int Y)> path) : base(x, y)
    {
        Path = new Queue<(int X, int Y)>(path); // Initialize the path queue
    }

    // Moves enemy towards the next waypoint
    public void MoveTowardsNextWaypoint()
    {
        // If already at final destination, no further movement
        if (IsAtFinalDestination) return;

        if (Path.Count == 0)
        {
            IsAtFinalDestination = true; // Mark the enemy as at the destination
            return;
        }

        var nextWaypoint = Path.Peek(); // Peek at the next waypoint

        // Move in the X direction
        if (X < nextWaypoint.X)
            X += Math.Min(Speed, nextWaypoint.X - X);
        else if (X > nextWaypoint.X)
            X -= Math.Min(Speed, X - nextWaypoint.X);

        // Move in the Y direction
        if (Y < nextWaypoint.Y)
            Y += Math.Min(Speed, nextWaypoint.Y - Y);
        else if (Y > nextWaypoint.Y)
            Y -= Math.Min(Speed, Y - nextWaypoint.Y);

        // If the enemy has reached the current waypoint, dequeue to move to the next
        if (X == nextWaypoint.X && Y == nextWaypoint.Y)
        {
            Path.Dequeue();
        }
    }

    // Check if enemy has reached its final destination (no more waypoints and the flag is set)
    public bool HasReachedDestination()
    {
        // The enemy has reached the destination if the path is empty and they have physically reached the final tile
        return IsAtFinalDestination;
    }
}
}
