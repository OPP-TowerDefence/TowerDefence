using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models
{
    public class GameState
    {
        public Map Map { get; } = new Map(100, 100);

        private readonly List<TowerTypes> _availableTowerTypes = Enum.GetValues(typeof(TowerTypes)).Cast<TowerTypes>().ToList();
        private readonly List<Player> _players = new();
        private readonly Queue<Tower> _towerPlacementQueue = new();
        private List<(int X, int Y)> _path1; // First path after the split
        private List<(int X, int Y)> _path2; // Second path after the split
        private IEnemyFactory _enemyFactory;
        private Random _random = new Random();
        private int _enemyCount = 0; // Track the number of enemies spawned

        public List<Player> Players => _players;
        public double TimeSinceLastSpawn { get; set; } = 0;

        public GameState()
        {
            _enemyFactory = RandomEnemyFactory();
            GenerateRandomPath(Map.Height, Map.Width); // Generate paths when the game starts
        }

        // Path strategy interface and implementations
        public interface IPathStrategy
        {
            Queue<(int X, int Y)> GetPath(GameState gameState);
        }

        public class FirstPathStrategy : IPathStrategy
        {
            public Queue<(int X, int Y)> GetPath(GameState gameState)
            {
                var (path1, _) = gameState.GetPaths();
                return new Queue<(int X, int Y)>(path1);
            }
        }

        public class SecondPathStrategy : IPathStrategy
        {
            public Queue<(int X, int Y)> GetPath(GameState gameState)
            {
                var (_, path2) = gameState.GetPaths();
                return new Queue<(int X, int Y)>(path2);
            }
        }

        public (List<(int X, int Y)> path1, List<(int X, int Y)> path2) GetPaths()
        {
            return (_path1, _path2);
        }

        private IEnemyFactory RandomEnemyFactory()
        {
            int enemyType = _random.Next(0, 3);
            return enemyType switch
            {
                0 => new FastEnemyFactory(),
                1 => new StrongEnemyFactory(),
                2 => new FlyingEnemyFactory(),
                _ => throw new Exception("Unknown enemy type"),
            };
        }

        public void SpawnEnemies()
    {
        IPathStrategy pathStrategy;
        if (_enemyCount % 20 < 10)
        {
            pathStrategy = new FirstPathStrategy();
        }
        else
        {
            pathStrategy = new SecondPathStrategy();
        }

        _enemyFactory = RandomEnemyFactory();
        Queue<(int X, int Y)> pathQueue = pathStrategy.GetPath(this); // Get the path as a Queue
        List<(int X, int Y)> pathList = pathQueue.ToList(); // Convert the Queue to a List
        Enemy enemy = _enemyFactory.CreateEnemy(0, 0, pathList); // Pass the path as a List
        Map.Enemies.Add(enemy);

        _enemyCount++; // Increment enemy count
}

        public void UpdateEnemies()
        {
            foreach (var enemy in Map.Enemies.ToList())
            {
                enemy.MoveTowardsNextWaypoint();
                if (enemy.HasReachedDestination())
                {
                    Map.Enemies.Remove(enemy);
                }
            }
        }

        public object GetMapTowers()
        {
            return Map.Towers.Select(t => new
            {
                t.X,
                t.Y,
                Category = t.Category.ToString(),
                Type = t.Type.ToString()
            }).ToList();
        }

        public object GetMapEnemies()
        {
            return Map.Enemies.Select(e => new
            {
                e.X,
                e.Y,
                e.Health,
                e.Speed
            }).ToList();
        }

        public object SendPath()
        {
             return new
            {
                Path1 = _path1.Select(point => new { X = point.X, Y = point.Y }).ToList(),
                Path2 = _path2.Select(point => new { X = point.X, Y = point.Y }).ToList()
            };
        }

        public bool IsOccupied(int x, int y)
        {
            return Map.Towers.Any(t => t.X == x && t.Y == y);
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Map.Width && y >= 0 && y < Map.Height;
        }

        private void PlaceTower(Tower tower)
        {
            if (!IsOccupied(tower.X, tower.Y) && IsValidPosition(tower.X, tower.Y))
            {
                Map.Towers.Add(tower);
            }
            else
            {
                Logger.Instance.LogError($"Unable to place tower at position ({tower.X},{tower.Y}). Position is either occupied or invalid.");
            }
        }

        public void ProcessTowerPlacements()
        {
            while (_towerPlacementQueue.Count > 0)
            {
                var tower = _towerPlacementQueue.Dequeue();
                PlaceTower(tower);
            }
        }

        public void QueueTowerPlacement(int x, int y, string connectionId, TowerCategories towerCategory)
        {
            if (IsValidPosition(x, y) && !IsOccupied(x, y))
            {
                var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);
                if (player != null)
                {
                    _towerPlacementQueue.Enqueue(player.CreateTower(x, y, towerCategory));
                    Logger.Instance.LogInfo($"Player {player.Username} queued a tower of category {towerCategory} at position ({x},{y}).");
                }
                else
                {
                    Logger.Instance.LogError($"Unable to queue tower. Player with connection ID {connectionId} was not found.");
                }
            }
        }

        public void AddPlayer(string username, string connectionId)
        {
            if (!_players.Any(p => p.ConnectionId == connectionId))
            {
                if (_availableTowerTypes.Count > 0)
                {
                    TowerTypes playerTowerType = _availableTowerTypes.First();
                    _players.Add(new Player(username, connectionId, playerTowerType));
                    _availableTowerTypes.Remove(playerTowerType);
                    Logger.Instance.LogInfo($"Player {username} joined the game.");
                }
                else
                {
                    Logger.Instance.LogError($"Player {username} could not be added. No available tower types left to assign to a new player.");
                }
            }
        }

        public void RemovePlayer(string connectionId)
        {
            var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (player != null)
            {
                _availableTowerTypes.Add(player.TowerType);
                _players.Remove(player);
                Logger.Instance.LogInfo($"Player {player.Username} left the game.");
            }
            else
            {
                Logger.Instance.LogError($"Could not remove player with connection ID {connectionId}. Player was not found.");
            }
        }

        public IEnumerable<object> GetActivePlayers()
        {
            return _players.Select(p => new
            {
                Username = p.Username,
                TowerType = p.TowerType.ToString()
            }).ToList();
        }

        public void GenerateRandomPath(int mapWidth, int mapHeight)
        {
            _path1 = new List<(int X, int Y)>();
            _path2 = new List<(int X, int Y)>();

            // Start at (0, 0)
            int currentX = 0;
            int currentY = 0;
            List<(int X, int Y)> commonPath = new List<(int X, int Y)> { (currentX, currentY) };

            // Determine the halfway point
            int halfway = (mapWidth + mapHeight) / 4; // You can tweak this to determine where the split should occur

            // First half of the path (before the split)
            while (commonPath.Count < halfway)
            {
                bool moveX = _random.Next(0, 2) == 0;

                if (moveX && currentX < mapWidth - 1)
                {
                    currentX += 1; // Move in X by 1 step
                }
                else if (currentY < mapHeight - 1)
                {
                    currentY += 1; // Move in Y by 1 step
                }

                // Add the new position to the common path
                if (!commonPath.Contains((currentX, currentY)))
                {
                    commonPath.Add((currentX, currentY));
                }
            }

            // Split: copy the common path to both _path1 and _path2
            _path1.AddRange(commonPath);
            _path2.AddRange(commonPath);

            // Generate the second half for _path1
            int splitX1 = currentX;
            int splitY1 = currentY;

            while (_path1.Last() != (mapWidth - 1, mapHeight - 1))
            {
                bool moveX = _random.Next(0, 2) == 0;

                if (moveX && splitX1 < mapWidth - 1)
                {
                    splitX1 += 1; // Move in X by 1 step
                }
                else if (splitY1 < mapHeight - 1)
                {
                    splitY1 += 1; // Move in Y by 1 step
                }

                if (!_path1.Contains((splitX1, splitY1)))
                {
                    _path1.Add((splitX1, splitY1));
                }
            }

            // Generate the second half for _path2
            int splitX2 = currentX;
            int splitY2 = currentY;

            while (_path2.Last() != (mapWidth - 1, mapHeight - 1))
            {
                bool moveX = _random.Next(0, 2) == 0;

                if (moveX && splitX2 < mapWidth - 1)
                {
                    splitX2 += 1; // Move in X by 1 step
                }
                else if (splitY2 < mapHeight - 1)
                {
                    splitY2 += 1; // Move in Y by 1 step
                }

                if (!_path2.Contains((splitX2, splitY2)))
                {
                    _path2.Add((splitX2, splitY2));
                }
            }

            // Ensure both paths end at the bottom-right corner
            if (_path1.Last() != (mapWidth - 1, mapHeight - 1))
            {
                _path1.Add((mapWidth - 1, mapHeight - 1));
            }
            if (_path2.Last() != (mapWidth - 1, mapHeight - 1))
            {
                _path2.Add((mapWidth - 1, mapHeight - 1));
            }
        }
    }
}
