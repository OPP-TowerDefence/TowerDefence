using TowerDefense.Models;

namespace TowerDefense.Interfaces
{
    public interface IMainObjectState
    {
        void DealDamage(MainObject mainobject, int damage);
        void Repair(MainObject mainobject, int heal);
        bool IsDestroyed();
        string GetStateGif();
    }
}
