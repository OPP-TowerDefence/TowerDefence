using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class FlameTowerFactory : ITowerFactory
    {
        public Tower CreateHeavyTower(int x, int y)
        {
            //return new HeavyFlameTower(x, y); 

            var builder = new FlameTowerBuilder(TowerCategories.Heavy); 
            builder.BuildBase(x, y);
            builder.AddWeapon(); 
            builder.AddArmor(); 
            return builder.GetResult(); 
        }

        public Tower CreateLongDistanceTower(int x, int y)
        {
            //return new LongDistanceFlameTower(x, y);

            var builder = new FlameTowerBuilder(TowerCategories.LongDistance);
            builder.BuildBase(x, y);
            builder.AddWeapon();
            builder.AddArmor();
            return builder.GetResult();
        }
    }
}
