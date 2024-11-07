using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{

    public class HealEffect(int healAmount) : ITileEffect
    {
        private readonly int _healAmount = healAmount;

        public void ApplyEffect(Enemy enemy)
        {
            enemy.IncreaseHealth(_healAmount);
        }
    }
}