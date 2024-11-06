using System.Collections.Generic;
using System.Linq;
using TowerDefense.Models;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Strategies
{
    public class ThreatAvoidanceStrategy : IPathStrategy
    {
        private List<PathPoint> selectedPath;
        private GameState lastGameState;

        public List<PathPoint> SelectPath(GameState gameState, Enemy enemy)
        {
            var map = gameState.Map;

            // Check if there are no turrets; if so, default to path0
            if (map.Towers.Count == 0)
            {
                selectedPath = map.Paths.Count > 0 ? map.Paths[0] : new List<PathPoint> { map.GetTile(enemy.X, enemy.Y) };
            }
            else if (gameState != lastGameState || selectedPath == null)
            {
                // If a turret was placed or no path is selected, find the least threatened path
                selectedPath = SelectLeastThreatenedPath(map);
                lastGameState = gameState;
            }

            // Check if we should switch to a better path
            selectedPath = CheckForBetterPath(map, enemy,gameState) ?? selectedPath;

            // Return the remaining portion of the current path from the enemy's position
            return GetRemainingPathFromCurrentPosition(selectedPath, enemy, gameState);
        }

        private List<PathPoint> SelectLeastThreatenedPath(Map map)
        {
            List<PathPoint> safestPath = null;
            int lowestThreatLevel = int.MaxValue;

            foreach (var path in map.Paths)
            {
                int pathThreatLevel = CalculatePathThreatLevel(map, path);

                if (pathThreatLevel < lowestThreatLevel)
                {
                    lowestThreatLevel = pathThreatLevel;
                    safestPath = path;
                }
            }

            return safestPath ?? map.Paths[0];
        }

        private int CalculatePathThreatLevel(Map map, List<PathPoint> path)
        {
            int totalThreatLevel = 0;
            foreach (var point in path)
            {
                totalThreatLevel += map.GetDefenseLevel(point.X, point.Y);
            }
            return totalThreatLevel;
        }

        private List<PathPoint> GetRemainingPathFromCurrentPosition(List<PathPoint> path, Enemy enemy, GameState gameState)
        {
            var currentTile = gameState.Map.GetTile(enemy.X, enemy.Y);
            var adjustedPath = new List<PathPoint>();

            if (path.Count == 0 || path[0].X != currentTile.X || path[0].Y != currentTile.Y)
            {
                adjustedPath.Add(currentTile);
            }
            adjustedPath.AddRange(path);

            var adjacentTiles = GetAdjacentTiles(enemy.X, enemy.Y, gameState);

            foreach (var waypoint in adjustedPath)
            {
                if (adjacentTiles.Any(tile => tile.X == waypoint.X && tile.Y == waypoint.Y))
                {
                    int startIndex = adjustedPath.IndexOf(waypoint);
                    return adjustedPath.Skip(startIndex).ToList();
                }
            }

            return null;
        }

        private List<PathPoint> CheckForBetterPath(Map map, Enemy enemy, GameState gameState)
{
    var currentTile = map.GetTile(enemy.X, enemy.Y);

    // Sort paths by their calculated threat level
    var sortedPaths = map.Paths
        .OrderBy(path => CalculatePathThreatLevel(map, path))
        .ToList();

    // Attempt to find the lowest-threat reachable path
    foreach (var path in sortedPaths)
    {
        // Skip checking the currently selected path if it’s already in use
        if (path == selectedPath) continue;

        // Check if this path is reachable from the enemy's current position
        var remainingPath = GetRemainingPathFromCurrentPosition(path, enemy, gameState);
        if (remainingPath != null)
        {
            // Found a reachable path with a lower threat level; use this one
            return remainingPath;
        }
    }

    // If no alternative reachable paths are found, stick with the current path
    return selectedPath;
}


        private List<PathPoint> GetAdjacentTiles(int x, int y, GameState gameState)
        {
            var adjacentTiles = new List<PathPoint>
            {
                gameState.Map.GetTile(x + 1, y),    // Right
                gameState.Map.GetTile(x, y + 1),    // Down
            };

            return adjacentTiles.Where(tile => tile != null && tile.Type != TileType.Turret).ToList();
        }
    }
}
