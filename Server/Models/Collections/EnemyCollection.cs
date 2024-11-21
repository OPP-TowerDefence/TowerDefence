using System.Collections;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Models.Collections
{
    public class EnemyCollection : IEnumerable<Enemy>
    {
        private readonly List<IEnemyComponent> _components = [];

        public int Count => _components.Count;

        public void Add(IEnemyComponent component)
        {
            _components.Add(component);
        }

        public IEnumerator<Enemy> GetEnumerator()
        {
            foreach (var component in _components)
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
            _components.Remove(component);
        }
    }
}
