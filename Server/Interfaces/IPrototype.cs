namespace TowerDefense.Interfaces
{
    public interface IPrototype<T>
    {
        T ShallowClone();
    }
}