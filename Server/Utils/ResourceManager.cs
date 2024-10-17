using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Utils
{
    public class ResourceManager
    {
        private readonly List<IResourceObserver> _observers = [];
        private int _resources;


        public int Resources
        {
            get => _resources;

            private set
            {
                _resources = value;
                NotifyResourceChanged();
            }
        }

        public void Attach(IResourceObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IResourceObserver observer)
        {
            _observers.Remove(observer);
        }

        public void OnEnemyDeath(Enemy enemy)
        {
            Resources += enemy.RewardValue;

            NotifyResourceChanged();
        }

        private void NotifyResourceChanged()
        {
            foreach (var observer in _observers)
            {
                observer.OnResourceChanged(_resources);
            }
        }
    }
}