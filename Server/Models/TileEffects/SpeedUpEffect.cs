using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{
    public class SpeedUpEffect : ITileEffect
    {
        private readonly int _speedIncrease;
        private readonly int _duration;

        public SpeedUpEffect(int speedIncrease, int duration)
        {
            _speedIncrease = speedIncrease;
            _duration = duration;
        }

        public void ApplyEffect(Enemy enemy)
        {
            enemy.ApplySpeedModifier(_speedIncrease, _duration);
        }
    }
}