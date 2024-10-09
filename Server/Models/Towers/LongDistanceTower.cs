using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public abstract class LongDistanceTower : Tower
    {
        public override TowerCategories Category => TowerCategories.LongDistance;
        public LongDistanceTower(int x, int y) : base(x, y)
        {
        }
    }
}
