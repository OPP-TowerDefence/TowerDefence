using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class FlameTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            return new HeavyFlameTower(x, y);
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            return new LongDistanceFlameTower(x, y);
        }
    }
}
