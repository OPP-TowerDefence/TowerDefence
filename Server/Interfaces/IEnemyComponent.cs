using TowerDefense.Models;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Interfaces
{
    public interface IEnemyComponent : IEnumerable<Enemy>
    {
        void MoveTowardsNextWaypoint(GameState gameState);
        bool HasReachedDestination();
        bool IsDead();
        void HandleDestination(MainObject mainObject, GameState gameState);
        void TakeDamage(int damage, GameState gameState);
    }
}
