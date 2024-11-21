using TowerDefense.Models;

namespace TowerDefense.Interfaces
{
    public interface IEnemyComponent
    {
        void MoveTowardsNextWaypoint(GameState gameState);
        bool HasReachedDestination();
        bool IsDead();
        void HandleDestination(MainObject mainObject, GameState gameState);
        void TakeDamage(int damage, GameState gameState);
    }
}
