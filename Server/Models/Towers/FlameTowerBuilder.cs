using System.Xml.Serialization;
using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class FlameTowerBuilder : TowerBuilder
    {
        public FlameTowerBuilder(TowerCategories category) : base(category)
        {
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

        public override FlameTowerBuilder AddWeapon()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Weapon = new Weapon("Flame cannon", 1, 10, 2);
            }
            else
            {
                _tower.Weapon = new Weapon("Flame Gun", 0, 10, 2);
            }
            return this;
        }

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

        public override Tower GetResult()
        {
            return _tower;
        }
    }
}
