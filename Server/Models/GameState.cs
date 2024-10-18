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
        private List<(int X, int Y)> _path; // Store the path specific to this game instance
        private IEnemyFactory _enemyFactory;
        private Random _random = new Random();

        public List<Player> Players => _players;

        public GameState()
        {
            _enemyFactory = RandomEnemyFactory();
            GenerateRandomPath(Map.Height,Map.Width); // Generate the path when the game starts
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
            var path = GetPath();

            // Create enemy with the path
            _enemyFactory = RandomEnemyFactory();
            Enemy enemy = _enemyFactory.CreateEnemy(0, 0, path); // Pass the path to the enemy
            Map.Enemies.Add(enemy);
        }

        public void UpdateEnemies()
        {
            foreach (var enemy in Map.Enemies.ToList())
            {
                enemy.MoveTowardsNextWaypoint();
                if (enemy.HasReachedDestination())
                {
                    enemy.MoveTowardsNextWaypoint();
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

        public List<(int X, int Y)> GetPath()
        {
            return _path;
        }
        public object SendPath()
        {
            return _path.Select(point => new { X = point.X, Y = point.Y }).ToList();
        }

       public void GenerateRandomPath(int mapWidth, int mapHeight)
{
    _path = new List<(int X, int Y)>();

    // Start at (0, 0)
    int currentX = 0;
    int currentY = 0;
    _path.Add((currentX, currentY));

    while (currentX < mapWidth - 1 || currentY < mapHeight - 1)
    {
        // Randomly decide whether to move in X or Y direction, but only by 1 step
        bool moveX = _random.Next(0, 2) == 0;

        if (moveX && currentX < mapWidth - 1)
        {
            currentX += 1; // Move in X by 1 step
        }
        else if (currentY < mapHeight - 1)
        {
            currentY += 1; // Move in Y by 1 step
        }

        // Add the new position to path only if it's different from the previous point
        if (!_path.Contains((currentX, currentY))) 
        {
            _path.Add((currentX, currentY));
        }
    }

    // Ensure the last point is the bottom-right corner
    if (_path.Last() != (mapWidth - 1, mapHeight - 1))
    {
        _path.Add((mapWidth - 1, mapHeight - 1));
    }
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
    }
}
