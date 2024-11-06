using TowerDefense.Models.Enemies;
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