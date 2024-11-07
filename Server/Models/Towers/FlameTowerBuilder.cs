using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class FlameTowerBuilder(TowerCategories category) : TowerBuilder(category)
    {
        public override FlameTowerBuilder AddArmor()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Armor = new Armor("Heat Shield", 10);
            }
            else
            {
                _tower.Armor = new Armor("Heat Shield", 5);
            }

            return this;
        }

        public override FlameTowerBuilder AddWeapon()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Weapon = new Weapon("Flame cannon", 4, 30, 2);
            }
            else
            {
                _tower.Weapon = new Weapon("Flame Gun", 2, 50, 4);
            }

            return this;
        }

        public override FlameTowerBuilder BuildBase(int x, int y)
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower = new HeavyFlameTower(x, y);
            }
            else
            {
                _tower = new LongDistanceFlameTower(x, y);
            }

            return this;
        }
     
        public override Tower GetResult()
        {
            return _tower;
        }
    }
}
