using System.Xml.Serialization;
using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class IceTowerBuilder : TowerBuilder
    {
        public IceTowerBuilder(TowerCategories catergory) : base(catergory)
        {
        }

        public override void BuildBase(int x, int y)
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower = new HeavyIceTower(x, y);
            }
            else
            {
                _tower = new LongDistanceIceTower(x, y);
            }
        }

        public override void AddWeapon()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Weapon = new Weapon("Ice Cannon", 40, 2, 1);
            }
            else
            {
                _tower.Weapon = new Weapon("Ice Gun", 5, 10, 5);
            }
        }

        public override void AddArmor()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Armor = new Armor("Ice Barrier", 20);
            }
            else
            {
                _tower.Armor = new Armor("Ice Barrier", 10);
            }
        }

        public override Tower GetResult()
        {
            return _tower;
        }
    }
}
