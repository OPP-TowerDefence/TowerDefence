namespace TowerDefense.Models;
public class GameState
{
    public Map Map { get; } = new Map(10, 10);
    public List<Tower> Towers { get; set; } = [];

    private readonly Queue<(int x, int y)> _towerPlacementQueue = new();

    public object GetMap()
    {
        return Towers
            .Select(t => new 
            { 
                t.X,
                t.Y 
            })
            .ToList();
    }

    public bool IsOccupied(int x, int y)
    {
        return Towers.Any(t => t.X == x && t.Y == y);
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < Map.Width && y >= 0 && y < Map.Height;
    }

    private void PlaceTower(int x, int y)
    {
        if (!IsOccupied(x, y) && IsValidPosition(x, y))
        {
            Towers.Add(new Tower { X = x, Y = y });
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
