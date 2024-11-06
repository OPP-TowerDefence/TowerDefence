using TowerDefense.Models.Enemies;
using TowerDefense.Models;
public interface IPathStrategy
{
     List<PathPoint> SelectPath(GameState gameState, Enemy enemy);
}
