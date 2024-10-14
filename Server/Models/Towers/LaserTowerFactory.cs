using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class LaserTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            //return new HeavyLaserTower(x, y);

            var builder = new LaserTowerBuilder(TowerCategories.Heavy);
            builder.BuildBase(x, y);
            builder.AddWeapon();
            builder.AddArmor();
            return builder.GetResult();
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            //return new LongDistanceLaserTower(x, y);

            var builder = new LaserTowerBuilder(TowerCategories.LongDistance);
            builder.BuildBase(x, y);
            builder.AddWeapon();
            builder.AddArmor();
            return builder.GetResult();
        }
    }
}
