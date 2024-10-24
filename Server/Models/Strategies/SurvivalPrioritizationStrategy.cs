using System.Collections.Generic;
using System.Linq;
using TowerDefense.Models.Enemies;
using TowerDefense.Models;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Strategies
{
    public class SurvivalPrioritizationStrategy : IPathStrategy
    {
        public Queue<PathPoint> GetPath(GameState gameState, Enemy enemy)
        {
            var paths = gameState.GetPaths();

            // Find the path that has the closest PinkHealth tile
            var bestPath = paths
                .Select(path => new
                {
                    Path = path,
                    ClosestPinkHealthDistance = path
                        .Select(point => GetDistanceToPinkHealthTile(gameState, point))
                        .Where(distance => distance >= 0) // Only consider valid distances
                        .DefaultIfEmpty(int.MaxValue) // Use int.MaxValue if no pink tiles are reachable
                        .Min() // Get the minimum distance
                })
                .OrderBy(distanceInfo => distanceInfo.ClosestPinkHealthDistance) // Prioritize by closest distance
                .FirstOrDefault()?.Path; // Get the best path

            return bestPath != null ? new Queue<PathPoint>(bestPath) : new Queue<PathPoint>();
        }

        private int GetDistanceToPinkHealthTile(GameState gameState, PathPoint point)
        {
            // Find the nearest PinkHealth tile from the given point
            foreach (var enemyPoint in gameState.Map.Enemies) // Assuming Map.Enemies contains the tiles on the map
            {
                if (gameState.Map.GetTileType(enemyPoint.X, enemyPoint.Y) == TileType.PinkHealth)
                {
                    int distance = Math.Abs(point.X - enemyPoint.X) + Math.Abs(point.Y - enemyPoint.Y);
                    return distance; // Return the calculated distance
                }
            }
            return -1; // Return -1 if there are no PinkHealth tiles
        }
    }
}
