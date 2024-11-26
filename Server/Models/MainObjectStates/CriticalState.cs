using TowerDefense.Interfaces;

namespace TowerDefense.Models.MainObjectStates
{
    public class CriticalState : IMainObjectState
    {
        public static readonly int MinHealth = 0;

        public void DealDamage(MainObject mainobject, int damage)
        {
            mainobject.Health = Math.Max(mainobject.Health - damage, 0);

            switch (mainobject.Health)
            {
                case var health when health <= MinHealth:
                    mainobject.ChangeState(new DestroyedState());
                    break;
                default:
                    break;
            }
        }

        public string GetStateGif()
        {
            return "/critical.gif";
        }

        public bool IsDestroyed()
        {
            return false;
        }

        public void Repair(MainObject mainobject, int heal)
        {
            mainobject.Health = Math.Min(mainobject.Health + heal, 100);

            switch (mainobject.Health)
            {
                case var health when health > NormalState.MinHealth:
                    mainobject.ChangeState(new NormalState());
                    break;
                case var health when health > DamagedState.MinHealth:
                    mainobject.ChangeState(new DamagedState());
                    break;
                default:
                    break;
            }

        }
    }
}
