using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{
    public class PinkTileEffect : ITileEffect
    {
        public void ApplyEffect(Enemy enemy)
        {
            enemy.IncreaseHealth(10);
        }
    }
}