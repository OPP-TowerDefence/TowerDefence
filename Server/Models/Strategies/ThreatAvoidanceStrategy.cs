using System.Collections.Generic;
using System.Linq;
using TowerDefense.Models.Enemies;
using TowerDefense.Models;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Strategies
{
    public class ThreatAvoidanceStrategy : IPathStrategy
    {
        public Queue<PathPoint> GetPath(GameState gameState, Enemy enemy)
        {
            var paths = gameState.GetPaths();

            // Find the path with the least number of towers in range
            var bestPath = paths
                .Select(path => new
                {
                    Path = path,
                    DefendedCount = path.Count(point => gameState.Map.IsTileDefended(point.X, point.Y))
                })
                .OrderBy(pathInfo => pathInfo.DefendedCount) // Prioritize by the least defended
                .FirstOrDefault()?.Path; // Get the best path

            return bestPath != null ? new Queue<PathPoint>(bestPath) : new Queue<PathPoint>();
        }
    }
}
