using System.Collections.Generic;
using System.Linq;
using TowerDefense.Models.Enemies;
using TowerDefense.Models;
using TowerDefense.Utils;

namespace TowerDefense.Models.Strategies
{
    public class GroupProtectionStrategy : IPathStrategy
    {
        private const int GroupSize = 4; // Number of enemies needed to group
        private const int FormationWidth = 2; // Width of the square formation

        public Queue<PathPoint> GetPath(GameState gameState, Enemy enemy)
        {
            var paths = gameState.GetPaths();

            // Check if there are any turrets on the map
            bool hasTurrets = gameState.Map.Towers.Any();

            // If there are no turrets, behave like SpeedPrioritizationStrategy
            if (!hasTurrets)
            {
                return GetFastestPath(gameState, paths);
            }

            // If turrets are present, check if the enemy can wait for more allies
            if (IsOutsideTurretRange(gameState, enemy) && CanWaitForGroup(gameState, enemy))
            {
                Logger.Instance.LogInfo($"Enemy at ({enemy.X}, {enemy.Y}) is grouping with allies.");
                return GetFormationPath(gameState, enemy);
            }

            // Find the path where the most allies are currently present
            var bestPath = paths
                .OrderByDescending(path => gameState.Map.Enemies.Count(e => e.X == path.Last().X && e.Y == path.Last().Y))
                .FirstOrDefault();

            // If no path was found, log a warning and return an empty queue
            if (bestPath == null)
            {
                return new Queue<PathPoint>(); // Could also consider returning a default path if needed
            }

            // Convert the bestPath (List<PathPoint>) to a Queue<PathPoint>
            return new Queue<PathPoint>(bestPath);
        }

        private Queue<PathPoint> GetFastestPath(GameState gameState, List<List<PathPoint>> paths)
        {
            // Find the path with the fewest Mud tiles for movement speed
            var fastestPath = paths
                .OrderBy(path =>
                    path.Count(point => gameState.Map.GetTileType(point.X, point.Y) == TileType.Mud))
                .FirstOrDefault();

            return fastestPath != null ? new Queue<PathPoint>(fastestPath) : new Queue<PathPoint>();
        }

        private bool IsOutsideTurretRange(GameState gameState, Enemy enemy)
        {
            // Check if the enemy is outside the range of any turrets
            foreach (var tower in gameState.Map.Towers)
            {
                if (tower.IsInRange(enemy.X, enemy.Y))
                {
                    return false; // Enemy is in turret range
                }
            }
            return true; // Enemy is outside turret range
        }

        private bool CanWaitForGroup(GameState gameState, Enemy enemy)
        {
            // Check if the path is a two-wide path
            var twoWidePath = gameState.GetPaths().Any(path => path.Count > 1 && !gameState.Map.IsTileDefended(path[0].X, path[0].Y));

            // Check the number of waiting enemies on the same path
            var waitingEnemiesCount = gameState.Map.Enemies.Count(e => e.CurrentStrategy is GroupProtectionStrategy && e.Path.Count > 0 && e.Path.Peek().X == enemy.X && e.Path.Peek().Y == enemy.Y);

            // If there are enough waiting enemies and it's safe, allow the enemy to wait
            return twoWidePath && waitingEnemiesCount < GroupSize;
        }

        private Queue<PathPoint> GetFormationPath(GameState gameState, Enemy enemy)
        {
            // Assume the enemy's current position is the center of the formation
            var formationPath = new List<PathPoint>();
            int startX = enemy.X;
            int startY = enemy.Y;

            // Define offsets for a square formation (2x2)
            (int xOffset, int yOffset)[] offsets = new (int, int)[]
            {
                (0, 0), // Enemy 1 (top-left)
                (1, 0), // Enemy 2 (top-right)
                (0, 1), // Enemy 3 (bottom-left)
                (1, 1)  // Enemy 4 (bottom-right)
            };

            foreach (var (xOffset, yOffset) in offsets)
            {
                int formationX = startX + xOffset;
                int formationY = startY + yOffset;

                // Ensure the position is valid and not occupied
                if (gameState.IsValidPosition(formationX, formationY) && !gameState.Map.IsOccupied(formationX, formationY))
                {
                    formationPath.Add(new PathPoint(formationX, formationY, TileType.Normal)); // Using Normal as placeholder
                }
            }

            // Return the path points for the formation
            return new Queue<PathPoint>(formationPath);
        }
    }
}
