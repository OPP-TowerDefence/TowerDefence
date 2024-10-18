using TowerDefense.Models;
public interface IPathStrategy
{
    Queue<(int X, int Y)> GetPath(GameState gameState);
}