using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Utils;

namespace TowerDefense.Models.Mediator
{
    public class ResourceManager
    {
        private readonly List<IResourceObserver> _observers = [];

        private int _resources;

        private IMediator? _mediator;

        public ResourceManager(IMediator? mediator)
        {
            _mediator = mediator;
            _resources = 100;
        }

        public void SetMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Attach(IResourceObserver observer)
        {
            _observers.Add(observer);
        }

        public bool OnTowerUpgrade(int amount)
        {
            if (_mediator == null)
            {
                Logger.Instance.LogError("Mediator is not set in ResourceManager");
                return false;
            }

            if (CanAfford(amount))
            {
                _resources -= amount;

                NotifyResourcesSpent(amount);

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
            if (_mediator == null)
            {
                Logger.Instance.LogError("Mediator is not set in ResourceManager");
                return;
            }

            _resources += enemy.Flyweight.RewardValue;

            _mediator.Notify(this, AchievementMediatorEvents.EnemyKilled.ToString(), 1);

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

        public bool CanAfford(int cost)
        {
            return _resources >= cost;
        }

        public void OnAchievementReached(int reward)
        {
            _resources += reward;
            NotifyResourceChanged();
        }

        public void OnTowerPurchase(int cost)
        {
            _resources -= cost;
            NotifyResourcesSpent(cost);
            NotifyResourceChanged();
        }

        private void NotifyResourcesSpent(int resources)
        {
            if (_mediator == null)
            {
                Logger.Instance.LogError("Mediator is not set in ResourceManager");
                return;
            }

            _mediator.Notify(this, AchievementMediatorEvents.ResourceSpent.ToString(), resources);
        }
    }
}