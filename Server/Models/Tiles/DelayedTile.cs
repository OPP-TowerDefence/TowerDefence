using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Tiles
{
    public class DelayedApplication : IEffectApplicationType
    {
        private readonly int _delay;

        public DelayedApplication(int delay)
        {
            _delay = delay;
        }

        public void ApplyEffect(ITileEffect effect, Enemy enemy)
        {
            enemy.ScheduleEffect(effect, _delay);
        }
    }
}