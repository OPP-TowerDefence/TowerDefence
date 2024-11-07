using TowerDefense.Models.Enemies;

namespace TowerDefense.Interfaces
{
    public interface ITileEffect
    {
        void ApplyEffect(Enemy enemy);
    }
}