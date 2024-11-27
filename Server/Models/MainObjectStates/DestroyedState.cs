using TowerDefense.Interfaces;
using TowerDefense.Utils;

namespace TowerDefense.Models.MainObjectStates
{
    public class DestroyedState : IMainObjectState
    {
        public static readonly int MinHealth = 0;
        public void DealDamage(MainObject mainobject, int damage)
        {
            return;
        }

        public int GenerateResources()
        {
            return 0;
        }

        public string GetStateGif()
        {
            return "/destroyed.gif";
        }

        public bool IsDestroyed()
        {
            return true;
        }

        public void Repair(MainObject mainobject, int health)
        {
            return;
        }
    }
}
