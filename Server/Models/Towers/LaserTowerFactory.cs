using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class LaserTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            return new HeavyLaserTower(x, y);
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            return new LongDistanceLaserTower(x, y);
        }
    }
}
