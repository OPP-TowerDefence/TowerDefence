using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;

public class ResourceManager
{
    private List<IResourceObserver> _observers = new();
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

    private void NotifyResourceChanged()
    {
        foreach (var observer in _observers)
        {
            observer.OnResourceChanged(_resources);
        }
    }

    public void OnEnemyDied(Enemy enemy)
    {
        Resources += enemy.RewardValue;

        NotifyResourceChanged();
    }
}
