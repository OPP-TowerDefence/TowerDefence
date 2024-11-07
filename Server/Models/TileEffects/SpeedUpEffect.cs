using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{
    public class SpeedUpEffect(int speedIncrease, int duration) : ITileEffect
    {
        private readonly int _speedIncrease = speedIncrease;
        private readonly int _duration = duration;

        public void ApplyEffect(Enemy enemy)
        {
            enemy.ApplySpeedModifier(_speedIncrease, _duration);
        }
    }
}