using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public abstract class HeavyTower(int x, int y) : Tower(x, y)
    {
        public override TowerCategories Category => TowerCategories.Heavy;
    }
}
