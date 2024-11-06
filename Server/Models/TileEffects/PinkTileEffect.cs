using TowerDefense.Models.Enemies;
namespace TowerDefense.Models.TileEffects
{
    public class PinkTileEffect : ITileEffect
    {
        public void ApplyEffect(Enemy enemy)
        {
            enemy.IncreaseHealth(30);
        }
    }
}