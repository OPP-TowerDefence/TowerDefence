using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class LaserTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            //return new HeavyLaserTower(x, y);

            return new LaserTowerBuilder(TowerCategories.Heavy).BuildBase(x, y).AddWeapon().AddArmor().GetResult();
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            //return new HeavyLaserTower(x, y);

            return new LaserTowerBuilder(TowerCategories.LongDistance).BuildBase(x, y).AddWeapon().AddArmor().GetResult();
        }
    }
}
