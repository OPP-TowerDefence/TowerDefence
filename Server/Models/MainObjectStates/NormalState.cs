using TowerDefense.Interfaces;

namespace TowerDefense.Models.MainObjectStates
{
    public class NormalState : IMainObjectState
    {
        public static readonly int MinHealth = 70;

        public void DealDamage(MainObject mainObject, int damage)
        {
            mainObject.Health = Math.Max(mainObject.Health - damage, 0);

            switch (mainObject.Health)
            {
                case var health when health <= CriticalState.MinHealth:
                    mainObject.ChangeState(new DestroyedState());
                    break;
                case var health when health <= DamagedState.MinHealth:
                    mainObject.ChangeState(new CriticalState());
                    break;
                case var health when health <= MinHealth:
                    mainObject.ChangeState(new DamagedState());
                    break;
                default:
                    break;
            }
        }

        public string GetStateGif()
        {
            return "/normal.gif";
        }

        public bool IsDestroyed()
        {
            return false;
        }

        public void Repair(MainObject mainObject, int heal)
        {
            mainObject.Health = Math.Min(mainObject.Health + heal, 100);
        }
    }
}
