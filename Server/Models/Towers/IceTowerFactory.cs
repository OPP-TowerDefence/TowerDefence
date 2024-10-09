using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class IceTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            return new HeavyIceTower(x, y);
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            return new LongDistanceIceTower(x, y);
        }
    }
}
