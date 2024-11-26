using TowerDefense.Interfaces;

namespace TowerDefense.Models.MainObjectStates
{
    public class DamagedState : IMainObjectState
    {
        public static readonly int MinHealth = 30;

        public void DealDamage(MainObject mainobject, int damage)
        {
            mainobject.Health = Math.Max(mainobject.Health - damage, 0);

            switch (mainobject.Health)
            {
                case var health when health <= CriticalState.MinHealth:
                    mainobject.ChangeState(new DestroyedState());
                    break;
                case var health when health <= MinHealth:
                    mainobject.ChangeState(new CriticalState());
                    break;
                default:
                    break;
            }
        }

        public string GetStateGif()
        {
            return "/damaged.gif";
        }

        public bool IsDestroyed()
        {
            return false;
        }

        public void Repair(MainObject mainobject, int health)
        { 
            mainobject.Health = Math.Min(mainobject.Health + health, 100);

            if (mainobject.Health > NormalState.MinHealth)
            {
                mainobject.ChangeState(new NormalState());
            }
        }
    }
}
