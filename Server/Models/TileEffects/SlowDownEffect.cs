using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{
    public class SlowDownEffect(int duration) : ITileEffect
    {
        private readonly int _duration = duration;

        public void ApplyEffect(Enemy enemy)
        {
            enemy.ApplySpeedModifier(-enemy.Speed, _duration);
        }
    }
}