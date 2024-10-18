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
        private List<List<(int X, int Y)>> _paths; // List to store all paths
        private IEnemyFactory _enemyFactory;
        private Random _random = new Random();
        private int _enemyCount = 0;

        public List<Player> Players => _players;
        public double TimeSinceLastSpawn { get; set; } = 0;

        public GameState()
        {
            _enemyFactory = RandomEnemyFactory();
            _paths = new List<List<(int X, int Y)>>(); // Initialize the paths list
            GenerateRandomPath(Map.Height, Map.Width); // Generate paths when the game starts
        }

        public class PathStrategy : IPathStrategy
        {
            private int _pathIndex;

            public PathStrategy(int pathIndex)
            {
                _pathIndex = pathIndex;
            }

            public Queue<(int X, int Y)> GetPath(GameState gameState)
            {
                return new Queue<(int X, int Y)>(gameState.GetPaths()[_pathIndex]);
            }
        }

        public List<List<(int X, int Y)>> GetPaths()
        {
            return _paths;
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
            int pathIndex = (_enemyCount / 10) % 4; // Rotate through 4 paths based on enemy count
            pathStrategy = new PathStrategy(pathIndex);

            _enemyFactory = RandomEnemyFactory();
            Queue<(int X, int Y)> pathQueue = pathStrategy.GetPath(this);
            List<(int X, int Y)> pathList = pathQueue.ToList();
            Enemy enemy = _enemyFactory.CreateEnemy(0, 0, pathList);
            Map.Enemies.Add(enemy);

            _enemyCount++;
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
            return _paths.Select(path => path.Select(point => new { X = point.X, Y = point.Y }).ToList()).ToList();
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
            _paths.Clear(); // Clear previous paths if any

            for (int i = 0; i < 4; i++) // Create 4 empty paths
            {
                _paths.Add(new List<(int X, int Y)>());
            }

            // Start at (0, 0)
            int currentX = 0;
            int currentY = 0;
            List<(int X, int Y)> commonPath = new List<(int X, int Y)> { (currentX, currentY) };

            // Determine the halfway point
            int halfway = (mapWidth + mapHeight) / 4;

            // First half of the path (before the split)
            while (commonPath.Count < halfway)
            {
                bool moveX = _random.Next(0, 2) == 0;

                if (moveX && currentX < mapWidth - 1)
                {
                    currentX += 1;
                }
                else if (currentY < mapHeight - 1)
                {
                    currentY += 1;
                }

                if (!commonPath.Contains((currentX, currentY)))
                {
                    commonPath.Add((currentX, currentY));
                }
            }

            // Split: copy the common path to all 4 paths
            foreach (var path in _paths)
            {
                path.AddRange(commonPath);
            }

            // Generate the second half for each path
            for (int i = 0; i < 4; i++)
            {
                GeneratePath(_paths[i], currentX, currentY, mapWidth, mapHeight);
            }
        }

        private void GeneratePath(List<(int X, int Y)> path, int startX, int startY, int mapWidth, int mapHeight)
        {
            int splitX = startX;
            int splitY = startY;

            while (path.Last() != (mapWidth - 1, mapHeight - 1))
            {
                bool moveX = _random.Next(0, 2) == 0;

                if (moveX && splitX < mapWidth - 1)
                {
                    splitX += 1;
                }
                else if (splitY < mapHeight - 1)
                {
                    splitY += 1;
                }

                if (!path.Contains((splitX, splitY)))
                {
                    path.Add((splitX, splitY));
                }
            }

            if (path.Last() != (mapWidth - 1, mapHeight - 1))
            {
                path.Add((mapWidth - 1, mapHeight - 1));
            }
        }
    }
}
