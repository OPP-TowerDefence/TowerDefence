using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class EnemyGroup : IEnemyComponent
    {
        private List<IEnemyComponent> _children = new();
        public IEnumerable<IEnemyComponent> Children => _children;

        public void MoveTowardsNextWaypoint(GameState gameState)
        {
            foreach (var child in _children.ToList())
            {
                child.MoveTowardsNextWaypoint(gameState);
            }
        }

        public bool HasReachedDestination()
        {
            return _children.All(child => child.HasReachedDestination());
        }

        public bool IsDead()
        {
            return _children.All(child => child.IsDead());
        }

        public void HandleDestination(MainObject mainObject, GameState gameState)
        {
            foreach (var child in _children.ToList())
            {
                child.HandleDestination(mainObject, gameState);
                Remove(child);
                Console.WriteLine($"Enemy {child} reached destination and was handled.");
            }

            if (IsDead())
            {
                gameState.Map.Enemies.Remove(this);
                Console.WriteLine($"Group {this} is empty and removed from map.");
            }
        }

        public void TakeDamage(int damage, GameState gameState)
        {
            foreach (var child in _children.ToList())
            {
                child.TakeDamage(damage, gameState);

                if (child.IsDead())
                {
                    Remove(child);
                    gameState.Map.Enemies.Remove(child);
                    Console.WriteLine($"Enemy {child} is dead and removed from group and map.");
                }
            }

            if (IsDead())
            {
                gameState.Map.Enemies.Remove(this);
                Console.WriteLine($"Group {this} is empty and removed from map.");
            }
        }

        public void Add(IEnemyComponent child) => _children.Add(child);
        public void Remove(IEnemyComponent child) => _children.Remove(child);
    }
}
