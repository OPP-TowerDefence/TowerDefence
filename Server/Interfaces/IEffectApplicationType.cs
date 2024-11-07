using TowerDefense.Models.Enemies;

namespace TowerDefense.Interfaces
{
    public interface IEffectApplicationType
    {
        void ApplyEffect(ITileEffect effect, Enemy enemy);
    }
}