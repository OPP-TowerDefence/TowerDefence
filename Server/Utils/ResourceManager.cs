using TowerDefense.Interfaces;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;

namespace TowerDefense.Utils
{
    public class ResourceManager
    {
        private readonly List<IResourceObserver> _observers = [];

        private int _resources;

        public void Attach(IResourceObserver observer)
        {
            _observers.Add(observer);
        }

        public bool DeductResources(int amount)
        {
            if (_resources >= amount)
            {
                _resources -= amount;
                NotifyResourceChanged();
                return true;
            }

            return false;
        }

        public void Detach(IResourceObserver observer)
        {
            _observers.Remove(observer);
        }

        public void OnEnemyDeath(Enemy enemy)
        {
            _resources += enemy.RewardValue;

            NotifyResourceChanged();
        }

        public void OnMainObjectGenerated(MainObject mainObject)
        {
            _resources += mainObject.GenerateResources();

            NotifyResourceChanged();
        }

        private void NotifyResourceChanged()
        {
            foreach (var observer in _observers)
            {
                observer.OnResourceChanged(_resources);
            }
        }

        public int GetResources()
        {
            return _resources;
        }
    }
}