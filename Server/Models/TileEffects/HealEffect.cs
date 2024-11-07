using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{

    public class HealEffect : ITileEffect
    {
        private readonly int _healAmount;

        public HealEffect(int healAmount)
        {
            _healAmount = healAmount;
        }

        public void ApplyEffect(Enemy enemy)
        {
            enemy.IncreaseHealth(_healAmount);
        }
    }
}