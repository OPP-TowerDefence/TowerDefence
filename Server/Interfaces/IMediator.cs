namespace TowerDefense.Interfaces
{
    public interface IMediator
    {
        void Notify(object sender, string eventCode, object data);
    }
}
