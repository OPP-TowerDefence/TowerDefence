namespace TowerDefense.Models;
public class GameState
{
    public Map Map { get; } = new Map(10, 10);

    private readonly Queue<(int x, int y)> _towerPlacementQueue = new();
    private readonly List<Player> _players = new();

    public List<Player> Players => _players;

    public object GetMap()
    {
        return Map.Towers
            .Select(t => new 
            { 
                t.X,
                t.Y 
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

    public void AddPlayer(Player player)
    {
        if (!_players.Any(p => p.ConnectionId == player.ConnectionId))
        {
            _players.Add(player);
        }
    }

    public void RemovePlayer(string connectionId)
    {
        var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);
        if (player != null)
        {
            _players.Remove(player);
        }
    }

    public IEnumerable<string> GetPlayersUsernames()
    {
        return _players.Select(p => p.Username);
    }
}
