using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{
    public class SlowDownEffect : ITileEffect
    {
        private readonly int _speedReduction;
        private readonly int _duration;

        public SlowDownEffect(int duration)
        {
            _duration = duration;
        }

        public void ApplyEffect(Enemy enemy)
        {
            enemy.ApplySpeedModifier(-enemy.Speed, _duration);
        }
    }
}