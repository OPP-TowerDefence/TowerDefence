using TowerDefense.Models.Enemies;
using TowerDefense.Models;
public interface IPathStrategy
{
     Queue<PathPoint> GetPath(GameState gameState, Enemy enemy);
}
