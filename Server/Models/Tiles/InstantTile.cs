using TowerDefense.Models.Enemies;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Tiles
{
    public class InstantApplication : IEffectApplicationType
    {
        public void ApplyEffect(ITileEffect effect, Enemy enemy)
        {
            effect.ApplyEffect(enemy);
        }
    }
}