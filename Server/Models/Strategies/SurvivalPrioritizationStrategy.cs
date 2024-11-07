using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Strategies
{
    public class SurvivalStrategy : IPathStrategy
    {
        public List<PathPoint> SelectPath(GameState gameState, Enemy enemy)
        {
            var paths = gameState.Map.Paths;
            if (paths == null || paths.Count == 0)
            {
                return null;
            }

            var healthTiles = gameState.Map.GetAllTilesOfType(TileType.PinkHealth);
            if (healthTiles.Count == 0)
            {
                return paths[0];
            }

            var rankedPaths = paths
                .OrderBy(path => GetManhattanDistanceToClosestHealthTile(path, healthTiles))
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

        private int GetManhattanDistanceToClosestHealthTile(List<PathPoint> path, List<PathPoint> healthTiles)
        {
            int minDistance = int.MaxValue;

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
