using System.Collections;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Collections
{
    public class EnemyCollection : IEnumerable<Enemy>
    {
        public List<IEnemyComponent> Components { get; set; } = [];

        public int Count => Components.Count;

        public EnemyCollection(IEnumerable<IEnemyComponent> enemies)
        {
            Components = enemies.ToList();
        }

        public EnemyCollection() { }

        public void Add(IEnemyComponent component)
        {
            Components.Add(component);
        }

        public IEnumerator<Enemy> GetEnumerator()
        {
            foreach (var component in Components)
            {
                foreach (var enemy in component)
                {
                    yield return enemy;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(IEnemyComponent component)
        {
            Components.Remove(component);
        }
    }
}
