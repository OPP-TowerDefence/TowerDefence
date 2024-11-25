using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class FlameTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            return new FlameTowerBuilder(TowerCategories.Heavy).BuildBase(x, y).AddWeapon().AddArmor().GetResult();       
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            return new FlameTowerBuilder(TowerCategories.LongDistance).BuildBase(x, y).AddWeapon().AddArmor().GetResult();
        }
    }
}
