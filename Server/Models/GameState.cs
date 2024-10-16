﻿using Microsoft.AspNetCore.SignalR;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models;

public class GameState
{
    public Map Map { get; } = new Map(10, 10);

    private readonly List<TowerTypes> _availableTowerTypes;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly List<Player> _players;
    private readonly ResourceManager _resourceManager;
    private readonly Queue<Tower> _towerPlacementQueue;

    private IEnemyFactory _enemyFactory;

    public List<Player> Players => _players;

    public GameState(IHubContext<GameHub> hubContext)
    {
        _availableTowerTypes = Enum
            .GetValues(typeof(TowerTypes))
            .Cast<TowerTypes>()
            .ToList();

        _enemyFactory = RandomEnemyFactory();
        _hubContext = hubContext;
        _players = [];
        _resourceManager = new();
        _towerPlacementQueue = new();
    }

    private IEnemyFactory RandomEnemyFactory()
    {
        Random rand = new Random();
        int enemyType = rand.Next(0, 3);

        switch (enemyType)
        {
            case 0:
                return new FastEnemyFactory();
            case 1:
                return new StrongEnemyFactory();
            case 2:
                return new FlyingEnemyFactory();
            default:
                Logger.Instance.LogError($"Unknown enemy type {enemyType} generated in RandomEnemyFactory.");
                throw new Exception("Unknown enemy type");
        }
    }

    public void SpawnEnemies()
    {
        _enemyFactory = RandomEnemyFactory();

        Enemy enemy = _enemyFactory.CreateEnemy(0,0);

        Map.Enemies.Add(enemy);
    }

    public void UpdateEnemies()
    {
        foreach (var enemy in Map.Enemies.ToList())
        {
            enemy.MoveTowardsTarget();

            if (enemy.HasReachedDestination())
            {
                Map.Enemies.Remove(enemy);
                enemy.TakeDamage(enemy.Health, _resourceManager);
            }
        }
    }

    public object GetMapTowers()
    {
        return Map.Towers
            .Select(t => new 
            { 
                t.X,
                t.Y,
                Category = t.Category.ToString(),
                Type = t.Type.ToString()
            })
            .ToList();
    }

    public object GetMapEnemies()
    {
        return Map.Enemies
            .Select(e => new
            {
                e.X,
                e.Y,
                e.Health,
                e.Speed
            })
            .ToList();
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
            var tower  = _towerPlacementQueue.Dequeue();

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

                var player = new Player(username, connectionId, playerTowerType, _hubContext);

                _players.Add(player);
                _resourceManager.Attach(player);

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
            _resourceManager.Detach(player);

            Logger.Instance.LogInfo($"Player {player.Username} left the game.");
        }
        else
        {
            Logger.Instance.LogError($"Could not remove player with connection ID {connectionId}. Player was not found.");
        }
    }

    public IEnumerable<object> GetActivePlayers()
    {
        return _players
            .Select(p => new
            {
                Username = p.Username,
                TowerType = p.TowerType.ToString()
            })
            .ToList();
    }
}
