using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Strategies
{
    public class ShadowStrategy : IPathStrategy
    {
        public StrongEnemy targetStrongEnemy = null;
        private Queue<PathPoint> delayedPath = new Queue<PathPoint>();
        private int delayTiles = 1; // Number of tiles to delay by
        private int waitDuration = 3; // Number of turns to "wait" on the current tile

        public bool HasTarget() => targetStrongEnemy != null;

        public List<PathPoint> SelectPath(GameState gameState, Enemy enemy)
        {
            // If there is a delayed path to follow, return it
            return delayedPath.Count > 0 ? delayedPath.ToList() : new List<PathPoint>();
        }

        // Set the strong enemy target and calculate the full delayed path at this time
        public void SetTargetStrongEnemy(StrongEnemy strongEnemy)
        {
            if (targetStrongEnemy == null) // Only set the target once
            {
                targetStrongEnemy = strongEnemy;
                delayedPath.Clear(); // Clear any previous path

                // Copy the strong enemy's entire path with the delay
                if (strongEnemy.Path != null)
                {
                    var fullPath = strongEnemy.Path.ToList(); // Clone full path
                    delayedPath = ApplyDelay(fullPath, strongEnemy);
                }
            }
        }

        // Apply delay to the strong enemy’s full path, including "waiting" at certain points
        private Queue<PathPoint> ApplyDelay(List<PathPoint> fullPath, Enemy enemy)
{
    var delayedQueue = new Queue<PathPoint>();

    // Create the current tile PathPoint with enemy's current coordinates and tile type
    var currentTile = new PathPoint(enemy.X, enemy.Y, TileType.Normal);
    
    // Add the current tile multiple times to simulate waiting
    for (int w = 0; w < waitDuration; w++)
    {
        delayedQueue.Enqueue(currentTile);
    }

    // Skip the first 'delayTiles' tiles for the shadow delay effect
    for (int i = delayTiles; i < fullPath.Count; i++)
    {
        delayedQueue.Enqueue(fullPath[i]);
    }

    return delayedQueue;
}

        // Stop following but retain the delayed path for continued movement
        public void StopFollowing()
        {
            targetStrongEnemy = null; // Clear the target but retain the current delayed path
        }
    }
}
