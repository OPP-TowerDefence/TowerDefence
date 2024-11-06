using TowerDefense.Models.Enemies;
namespace TowerDefense.Models.TileEffects
{
    public class IceTileEffect : ITileEffect
    {
        public void ApplyEffect(Enemy enemy)
        {
            enemy.ApplySpeedModifier(4, 1);
        }
    }
}