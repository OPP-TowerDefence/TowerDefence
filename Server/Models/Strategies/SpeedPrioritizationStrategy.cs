using System.Collections.Generic;
using System.Linq;
using TowerDefense.Models.Enemies;
using TowerDefense.Models;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Strategies
{
    public class SpeedPrioritizationStrategy : IPathStrategy
    {
        public Queue<PathPoint> GetPath(GameState gameState, Enemy enemy)
        {
            var paths = gameState.GetPaths();

            // Find the path that has the most Ice tiles and the least Mud tiles
            var bestPath = paths
                .Select(path => new
                {
                    Path = path,
                    IceCount = path.Count(point => gameState.Map.GetTileType(point.X, point.Y) == TileType.Ice),
                    MudCount = path.Count(point => gameState.Map.GetTileType(point.X, point.Y) == TileType.Mud)
                })
                .OrderByDescending(p => p.IceCount) // Prioritize paths with the most Ice tiles
                .ThenBy(p => p.MudCount) // Then minimize the count of Mud tiles
                .FirstOrDefault()?.Path; // Get the best path

            return bestPath != null ? new Queue<PathPoint>(bestPath) : new Queue<PathPoint>();
        }
    }
}
