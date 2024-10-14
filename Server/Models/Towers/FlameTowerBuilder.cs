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

        public override void BuildBase(int x, int y)
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower = new HeavyFlameTower(x, y);
            }
            else
            {
                _tower = new LongDistanceFlameTower(x, y);
            }
        }

        public override void AddWeapon()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Weapon = new Weapon("Flame cannon", 30, 2, 3);
            }
            else
            {
                _tower.Weapon = new Weapon("Flame Gun", 3, 10, 7);
            }
        }

        public override void AddArmor()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Armor = new Armor("Heat Shield", 10);
            }
            else
            {
                _tower.Armor = new Armor("Heat Shield", 5);
            }
        }

        public override Tower GetResult()
        {
            return _tower;
        }
    }
}
