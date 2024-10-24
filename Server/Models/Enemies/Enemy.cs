using System;
using System.Collections.Generic;
using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models
{
    public abstract class Enemy : Unit
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public Queue<PathPoint> Path { get; set; } // Changed to Queue<PathPoint>

        private bool IsAtFinalDestination { get; set; } = false;

        public IPathStrategy CurrentStrategy { get; private set; }

        public Enemy(int x, int y, List<PathPoint> path) : base(x, y)
        {
            Path = new Queue<PathPoint>(path);
        }

        public void SetInitialStrategy(IPathStrategy strategy)
        {
            CurrentStrategy = strategy;
        }

        public void RetrievePath(GameState gameState)
        {
            if (CurrentStrategy != null)
            {
                Queue<PathPoint> pathQueue = CurrentStrategy.GetPath(gameState, this);
                Path = new Queue<PathPoint>(pathQueue);
            }
        }

        public void UpdateStrategyBasedOnState(GameState gameState)
{
    // Check proximity to the objective first for speed strategy
    if (IsNearObjective(gameState))
    {
        CurrentStrategy = new SpeedPrioritizationStrategy();
    }
    else if (Health < 25)
    {
        CurrentStrategy = new SurvivalPrioritizationStrategy();
    }
    // Check for pink tiles within 5 tiles for threat avoidance
    else if (!HasNearbyPinkTiles(gameState))
    {
        CurrentStrategy = new ThreatAvoidanceStrategy();
    }
    else
    {
        CurrentStrategy = new SpeedPrioritizationStrategy(); // Example default strategy
    }
}

private bool HasNearbyPinkTiles(GameState gameState)
{
    var objectiveTile = gameState.Map.GetObjectiveTile(); // Get the objective tile coordinates
    var nearbyTiles = new List<(int x, int y)>();

    // Check within a 5-tile radius
    for (int dx = -5; dx <= 5; dx++)
    {
        for (int dy = -5; dy <= 5; dy++)
        {
            int checkX = X + dx;
            int checkY = Y + dy;

            if (gameState.IsValidPosition(checkX, checkY) && 
                gameState.Map.GetTileType(checkX, checkY) == TileType.PinkHealth)
            {
                return true; // Found a pink tile within 5 tiles
            }
        }
    }
    
    return false; // No pink tiles found in range
}

        public void MoveTowardsNextWaypoint()
        {
            if (IsAtFinalDestination) return;

            if (Path.Count == 0)
            {
                IsAtFinalDestination = true;
                return;
            }

            var nextWaypoint = Path.Peek();

            // Move towards the next waypoint
            MoveTo(nextWaypoint);

            // If the enemy has reached the current waypoint, dequeue to move to the next
            if (X == nextWaypoint.X && Y == nextWaypoint.Y)
            {
                Path.Dequeue();
            }
        }

        private void MoveTo(PathPoint waypoint)
        {
            // Adjust movement logic based on the current position and the waypoint
            if (X < waypoint.X)
                X += Math.Min(Speed, waypoint.X - X);
            else if (X > waypoint.X)
                X -= Math.Min(Speed, X - waypoint.X);

            if (Y < waypoint.Y)
                Y += Math.Min(Speed, waypoint.Y - Y);
            else if (Y > waypoint.Y)
                Y -= Math.Min(Speed, Y - waypoint.Y);
        }

        public bool HasReachedDestination()
        {
            return IsAtFinalDestination;
        }

        public bool IsThreatened(GameState gameState)
        {
            // Check if there are any towers in range that can attack the enemy
            foreach (var tower in gameState.Map.Towers)
            {
                if (tower.IsInRange(X, Y))
                {
                    return true; // The enemy is threatened by a tower
                }
            }
            return false; // No threats detected
        }

        public bool IsNearObjective(GameState gameState)
        {
            var objectiveTile = gameState.Map.GetObjectiveTile(); // Assume this method retrieves the objective tile
            int distanceToObjective = Math.Abs(X - objectiveTile.X) + Math.Abs(Y - objectiveTile.Y);
            return distanceToObjective < 15; // Considered "near" if within a distance of 5 tiles
        }
    }
}
