using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class IceTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            //return new HeavyIceTower(x, y);

            var builder = new IceTowerBuilder(TowerCategories.Heavy);
            builder.BuildBase(x, y);
            builder.AddWeapon();
            builder.AddArmor();
            return builder.GetResult();
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            //return new LongDistanceIceTower(x, y);

            var builder = new IceTowerBuilder(TowerCategories.LongDistance);
            builder.BuildBase(x, y);
            builder.AddWeapon();
            builder.AddArmor();
            return builder.GetResult();
        }
    }
}
