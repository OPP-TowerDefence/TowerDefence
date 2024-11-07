using TowerDefense.Models.Enemies;
using TowerDefense.Models;

namespace TowerDefense.Interfaces
{
     public interface IPathStrategy
     {
          List<PathPoint> SelectPath(GameState gameState, Enemy enemy);
     }
}
