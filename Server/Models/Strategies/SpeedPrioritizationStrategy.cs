using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Strategies
{
    public class SpeedPrioritizationStrategy : IPathStrategy
    {
        public List<PathPoint> SelectPath(GameState gameState, Enemy enemy)
        {
            var paths = gameState.Map.Paths;
            if (paths == null || paths.Count == 0) return null;

            var objectiveTile = gameState.Map.GetObjectiveTile();

            // Rank paths based on speed optimization criteria
            var rankedPaths = paths
                .OrderByDescending(path => CountTileType(path, TileType.Ice))        // Maximize Ice tiles for speed
                .ThenBy(path => CountTileType(path, TileType.Mud))                   // Minimize Mud tiles that slow down
                .ThenBy(path => CalculatePathDistance(path, objectiveTile))          // Shortest distance as a tiebreaker
                .ToList();

            // Try each path in the order of ranking until a reachable one is found
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

        private int CalculatePathDistance(List<PathPoint> path, PathPoint objectiveTile)
        {
            return path.Count > 0 ? CalculateManhattanDistance(path.Last(), objectiveTile) : int.MaxValue;
        }

        private int CountTileType(List<PathPoint> path, TileType tileType)
        {
            return path.Count(point => point.Type == tileType);
        }

        private int CalculateManhattanDistance(PathPoint point1, PathPoint point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
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
