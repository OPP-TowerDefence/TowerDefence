using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class IceTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            //return new HeavyIceTower(x, y);

            return new IceTowerBuilder(TowerCategories.Heavy).BuildBase(x, y).AddWeapon().AddArmor().GetResult();
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            //return new HeavyIceTower(x, y);

            return new IceTowerBuilder(TowerCategories.LongDistance).BuildBase(x, y).AddWeapon().AddArmor().GetResult();
        }
    }
}
