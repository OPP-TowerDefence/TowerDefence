using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models
{
    public class GameState
    {
        public Map Map { get; } = new Map(100, 100);

        private readonly List<TowerTypes> _availableTowerTypes = Enum.GetValues(typeof(TowerTypes)).Cast<TowerTypes>().ToList();
        private readonly List<Player> _players = new();
        private readonly Queue<Tower> _towerPlacementQueue = new();
        private List<List<PathPoint>> _paths; // Updated to use PathPoint
        private IEnemyFactory _enemyFactory;
        private Random _random = new Random();
        private int _enemyCount = 0;
        private List<Enemy> _waitingEnemies = new();

        public List<Player> Players => _players;
        public double TimeSinceLastSpawn { get; set; } = 0;

        public GameState()
        {
            _enemyFactory = RandomEnemyFactory();
            _paths = new List<List<PathPoint>>(); // Initialize as list of lists of PathPoint
            GenerateRandomPath(Map.Height, Map.Width);
        }

        public List<List<PathPoint>> GetPaths()
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
    _enemyFactory = RandomEnemyFactory();
    // Create an enemy
    Enemy enemy = _enemyFactory.CreateEnemy(0, 0, new List<PathPoint>());

    // Retrieve the path based on the assigned strategy
    enemy.RetrievePath(this);

    // Add the enemy to the map if it has a valid path
    if (enemy.Path.Count > 0)
    {
        Map.Enemies.Add(enemy);
        _enemyCount++;
    }
    else
    {
        Logger.Instance.LogError("No valid path found for the enemy.");
    }
}
        public void UpdateEnemies()
        {
    // List to keep track of waiting enemies
    var waitingEnemies = new List<Enemy>();

    foreach (var enemy in Map.Enemies.ToList())
    {
        enemy.UpdateStrategyBasedOnState(this);

        // Check if the enemy is waiting based on the GroupProtectionStrategy
        if (enemy.CurrentStrategy is GroupProtectionStrategy && enemy.Path.Count == 0)
        {
            waitingEnemies.Add(enemy); // Add to waiting list
            continue; // Skip moving
        }

        // If enemy is not waiting, move towards the next waypoint
        enemy.MoveTowardsNextWaypoint();
        if (enemy.HasReachedDestination())
        {
            Map.Enemies.Remove(enemy);
        }
    }

    // Move waiting enemies if we have enough of them
    MoveWaitingEnemies(waitingEnemies);
}

private void MoveWaitingEnemies(List<Enemy> waitingEnemies)
{
    // Check if we have enough enemies waiting
    if (waitingEnemies.Count >= 4)
    {
        // Move each waiting enemy
        foreach (var waitingEnemy in waitingEnemies)
        {
            waitingEnemy.MoveTowardsNextWaypoint();

            // Check if the enemy has reached its destination
            if (waitingEnemy.HasReachedDestination())
            {
                Map.Enemies.Remove(waitingEnemy); // Remove from the map if reached
            }
        }

        // Clear the waiting enemies list after moving them
        waitingEnemies.Clear();
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
            e.Speed,
            Type = e.GetType().Name
        }).ToList();
    }

        public object SendPath()
        {
            return _paths.Select(path => path.Select(point => new 
            {
                X = point.X, 
                Y = point.Y,
                Type = point.Type.ToString() // Include tile type as a string
            }).ToList()).ToList();
        }

        public bool IsOccupied(int x, int y)
        {
            return Map.Towers.Any(t => t.X == x && t.Y == y);
        }

        public bool IsValidPosition(int x, int y)
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
        _paths.Add(new List<PathPoint>());
    }

    var (startX, startY) = (0,0); // Get the actual starting tile coordinates
    var (objectiveX, objectiveY) = Map.GetObjectiveTile(); // Get the objective tile coordinates

    int totalPathLength = Math.Abs(objectiveX - startX) + Math.Abs(objectiveY - startY);
    int first33PercentLength = (int)(totalPathLength * 0.33); // First 33% of the path should be normal tiles
    int sharedPathLength = 10; // The first 10 tiles are shared

    // Generate the shared starting path for the first 10 tiles
    List<PathPoint> sharedPath = new List<PathPoint>();
    int currentX = startX;
    int currentY = startY;

    for (int i = 0; i < sharedPathLength; i++) // First 10 shared tiles
    {
        // Force all tiles to be TileType.Normal within the first 33% of the path
        TileType tileType = i < first33PercentLength ? TileType.Normal : TileType.Normal; // Ensuring normal

        if (currentX < objectiveX)
        {
            currentX++;
        }
        else if (currentY < objectiveY)
        {
            currentY++;
        }

        sharedPath.Add(new PathPoint(currentX, currentY, tileType));
    }

    // Assign the shared path to all 4 paths
    foreach (var path in _paths)
    {
        path.AddRange(sharedPath);
    }

    // Now diverge the paths after the shared segment
    for (int pathIndex = 0; pathIndex < 4; pathIndex++)
    {
        currentX = sharedPath.Last().X;
        currentY = sharedPath.Last().Y;

        HashSet<(int, int)> occupiedPositions = new HashSet<(int, int)>(
            _paths[pathIndex].Select(p => (p.X, p.Y))
        );

        bool divergedFromShared = false;
        while (currentX < objectiveX || currentY < objectiveY)
        {
            // Randomly decide to move horizontally or vertically, while avoiding overlap
            bool moveHorizontally = _random.Next(0, 2) == 0;

            if (moveHorizontally && currentX < objectiveX)
            {
                currentX++;
            }
            else if (currentY < objectiveY)
            {
                currentY++;
            }

            // Ensure the path does not overlap with other diverged paths
            if (!occupiedPositions.Contains((currentX, currentY)))
            {
                TileType tileType;

                // Ensure the first 33% of the path is normal tiles
                int pathLengthSoFar = _paths[pathIndex].Count + sharedPath.Count;
                if (pathLengthSoFar < first33PercentLength)
                {
                    tileType = TileType.Normal; // Force normal for first 33%
                }
                else
                {
                    tileType = DetermineTileType(); // After 33%, use random tiles
                }

                _paths[pathIndex].Add(new PathPoint(currentX, currentY, tileType));
                occupiedPositions.Add((currentX, currentY));
            }
        }

        // Ensure the objective tile is added at the end of each path
        if (!_paths[pathIndex].Any(p => p.X == objectiveX && p.Y == objectiveY))
        {
            _paths[pathIndex].Add(new PathPoint(objectiveX, objectiveY, TileType.Objective));
        }
    }
}

private TileType DetermineTileType()
{
    // Logic to determine tile type, can be customized
    double randomValue = _random.NextDouble();
    if (randomValue < 0.1)
    {
        return TileType.Ice;
    }
    else if (randomValue < 0.2)
    {
        return TileType.Mud;
    }
    else if (randomValue < 0.3)
    {
        return TileType.PinkHealth;
    }
    return TileType.Normal; // Default to normal if not special
}


    }
}
