using TowerDefense.Enums;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models;

public class GameState
{
    public Map Map { get; } = new Map(10, 10);

    private readonly Queue<Tower> _towerPlacementQueue = new();
    private readonly List<Player> _players = new();
    public List<Player> Players => _players;
    private readonly List<TowerTypes> _availableTowerTypes = Enum.GetValues(typeof(TowerTypes)).Cast<TowerTypes>().ToList();

    public object GetMap()
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

            if(player != null)
            {
                var tower = player.CreateTower(x, y, towerCategory);
                _towerPlacementQueue.Enqueue(tower);
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

                var player = new Player(username, connectionId, playerTowerType);
                _players.Add(player);

                _availableTowerTypes.Remove(playerTowerType);
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


    public Player? GetPlayer(string connectionId)
    {
        return _players.FirstOrDefault(p => p.ConnectionId == connectionId);
    }
}
