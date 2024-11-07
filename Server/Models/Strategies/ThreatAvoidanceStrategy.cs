using TowerDefense.Enums;
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

            if (map.Towers.Count == 0)
            {
                selectedPath = map.Paths.Count > 0 ? map.Paths[0] : [map.GetTile(enemy.X, enemy.Y)];
            }
            else if (gameState != lastGameState || selectedPath == null)
            {
                selectedPath = SelectLeastThreatenedPath(map);
                lastGameState = gameState;
            }

            selectedPath = CheckForBetterPath(map, enemy, gameState) ?? selectedPath;
            return GetRemainingPathFromCurrentPosition(selectedPath, enemy, gameState);
        }

        private List<PathPoint> SelectLeastThreatenedPath(Map map)
        {
            var safestPath = map.Paths[0];
            var lowestThreatLevel = int.MaxValue;

            foreach (var path in map.Paths)
            {
                var pathThreatLevel = CalculatePathThreatLevel(map, path);

                if (pathThreatLevel < lowestThreatLevel)
                {
                    lowestThreatLevel = pathThreatLevel;
                    safestPath = path;
                }
            }

            return safestPath;
        }

        private int CalculatePathThreatLevel(Map map, List<PathPoint> path)
        {
            var totalThreatLevel = 0;

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

            return [];
        }

        private List<PathPoint> CheckForBetterPath(Map map, Enemy enemy, GameState gameState)
        {
            var sortedPaths = map.Paths
                .OrderBy(path => CalculatePathThreatLevel(map, path))
                .ToList();

            foreach (var path in sortedPaths)
            {
                if (path == selectedPath) continue;

                var remainingPath = GetRemainingPathFromCurrentPosition(path, enemy, gameState);

                if (remainingPath != null && remainingPath.Count > 0)
                {
                    return remainingPath;
                }
            }

            return selectedPath;
        }

        private List<PathPoint> GetAdjacentTiles(int x, int y, GameState gameState)
        {
            return new List<PathPoint>
            {
                gameState.Map.GetTile(x + 1, y),
                gameState.Map.GetTile(x, y + 1),
            }
            .Where(tile => tile != null && tile.Type != TileType.Turret)
            .ToList();
        }
    }
}
