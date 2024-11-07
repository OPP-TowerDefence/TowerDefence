using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public abstract class LongDistanceTower(int x, int y) : Tower(x, y)
    {
        public override TowerCategories Category => TowerCategories.LongDistance;
    }
}
