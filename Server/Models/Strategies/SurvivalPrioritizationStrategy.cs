using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Strategies
{
    public class SurvivalStrategy : IPathStrategy
    {
        public List<PathPoint> SelectPath(GameState gameState, Enemy enemy)
        {
            var paths = gameState.Map.Paths;
            if (paths == null || paths.Count == 0) return null;

            // Get all health tiles on the map
            var healthTiles = gameState.Map.GetAllTilesOfType(TileType.PinkHealth);
            if (healthTiles.Count == 0) return paths[0]; // If no health tiles, fall back to any path

            // Sort paths by Manhattan distance to the closest health tile
            var rankedPaths = paths.OrderBy(path => GetManhattanDistanceToClosestHealthTile(path, healthTiles)).ToList();

            // Try each path in order of proximity until a reachable path is found
            foreach (var path in rankedPaths)
            {
                var reachablePath = GetRemainingPathFromCurrentPosition(path, enemy, gameState);
                if (reachablePath != null)
                {
                    return reachablePath; // Return the first reachable path found
                }
            }

            // If no reachable path is found, return the full best path as a fallback
            return rankedPaths[0];
        }

        private int GetManhattanDistanceToClosestHealthTile(List<PathPoint> path, List<PathPoint> healthTiles)
        {
            int minDistance = int.MaxValue;

            // Calculate Manhattan distance from each waypoint in the path to each health tile
            foreach (var waypoint in path)
            {
                foreach (var healthTile in healthTiles)
                {
                    int distance = Math.Abs(waypoint.X - healthTile.X) + Math.Abs(waypoint.Y - healthTile.Y);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
            }

            return minDistance;
        }

        private List<PathPoint> GetRemainingPathFromCurrentPosition(List<PathPoint> fullPath, Enemy enemy, GameState gameState)
        {
            // Get the current tile the enemy is on
            var currentTile = gameState.Map.GetTile(enemy.X, enemy.Y);

            // Ensure the path starts from the current tile
            var adjustedPath = new List<PathPoint>();
            if (fullPath.Count == 0 || fullPath[0].X != currentTile.X || fullPath[0].Y != currentTile.Y)
            {
                adjustedPath.Add(currentTile); // Add current tile if it's not the first in the path
            }
            adjustedPath.AddRange(fullPath); // Append the rest of the path

            // Get adjacent tiles to the enemy’s current position without backward movement
            var adjacentTiles = GetAdjacentTiles(enemy.X, enemy.Y, gameState);

            // Find the first reachable point on the adjusted path that is adjacent to the enemy
            foreach (var waypoint in adjustedPath)
            {
                if (adjacentTiles.Any(tile => tile.X == waypoint.X && tile.Y == waypoint.Y))
                {
                    int startIndex = adjustedPath.IndexOf(waypoint);
                    return adjustedPath.Skip(startIndex).ToList(); // Return the path starting from the reachable point
                }
            }

            // If no reachable entry point, return null to indicate this path is not reachable
            return null;
        }

        private List<PathPoint> GetAdjacentTiles(int x, int y, GameState gameState)
        {
            var adjacentTiles = new List<PathPoint>
            {
                gameState.Map.GetTile(x + 1, y),    // Right
                gameState.Map.GetTile(x, y + 1),    // Down
            };

            // Filter out null or invalid tiles (e.g., turrets)
            return adjacentTiles.Where(tile => tile != null && tile.Type != TileType.Turret).ToList();
        }
    }
}
