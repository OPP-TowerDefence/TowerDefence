﻿namespace TowerDefense.Models;
public class GameState
{
    public Map Map { get; } = new Map(10, 10);

    private readonly Queue<(int x, int y)> _towerPlacementQueue = new();

    public void SpawnEnemies()
    {
        IEnemyFactory factory;

        Random rand = new Random();
        int enemyType = rand.Next(0, 3);

        switch (enemyType)
        {
            case 0:
                factory = new FastEnemyFactory();
                break;
            case 1:
                factory = new StrongEnemyFactory();
                break;
            case 2:
                factory = new FlyingEnemyFactory();
                break;
            default:
                throw new Exception("Unknown enemy type");
        }

        Enemy enemy = factory.CreateEnemy();

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
            }
        }
    }

    public object GetMapTowers()
    {
        return Map.Towers
            .Select(t => new
            {
                t.X,
                t.Y
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

    private void PlaceTower(int x, int y)
    {
        if (!IsOccupied(x, y) && IsValidPosition(x, y))
        {
            Map.Towers.Add(new Tower { X = x, Y = y });
        }
    }

    public void ProcessTowerPlacements()
    {
        while (_towerPlacementQueue.Count > 0)
        {
            var (x, y) = _towerPlacementQueue.Dequeue();

            PlaceTower(x, y);
        }
    }

    public void QueueTowerPlacement(int x, int y)
    {
        if (IsValidPosition(x, y) && !IsOccupied(x, y))
        {
            _towerPlacementQueue.Enqueue((x, y));
        }
    }
}
