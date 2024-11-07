using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.TileEffects
{
    public class MudTileEffect : ITileEffect
    {
        public void ApplyEffect(Enemy enemy)
        {
            enemy.ApplySpeedModifier(-enemy.Speed, 2);
        }
    }
}