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
            var rankedPaths = paths
                .OrderByDescending(path => CountTileType(path, TileType.Ice))
                .ThenBy(path => CountTileType(path, TileType.Mud))
                .ThenBy(path => CalculatePathDistance(path, objectiveTile))
                .ToList();

            foreach (var path in rankedPaths)
            {
                var reachablePath = GetRemainingPathFromCurrentPosition(path, enemy, gameState);
                if (reachablePath != null)
                {
                    return reachablePath;
                }
            }
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
            var currentTile = gameState.Map.GetTile(enemy.X, enemy.Y);
            var adjustedPath = new List<PathPoint>();

            if (fullPath.Count == 0 || fullPath[0].X != currentTile.X || fullPath[0].Y != currentTile.Y)
            {
                adjustedPath.Add(currentTile);
            }
            adjustedPath.AddRange(fullPath);
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
        private List<PathPoint> GetAdjacentTiles(int x, int y, GameState gameState)
        {
            var adjacentTiles = new List<PathPoint>
            {
                gameState.Map.GetTile(x + 1, y),
                gameState.Map.GetTile(x, y + 1),
            };
            return adjacentTiles.Where(tile => tile != null && tile.Type != TileType.Turret).ToList();
        }
    }
}
