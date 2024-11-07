namespace TowerDefense.Interfaces
{
    public interface IPrototype<T>
    {
        T Clone();
    }
}