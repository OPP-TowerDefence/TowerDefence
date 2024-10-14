using System.Xml.Serialization;
using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class IceTowerBuilder : ITowerBuilder
    {
        private Tower _tower;
        private TowerCategories _category;

        public IceTowerBuilder(TowerCategories catergory)
        {
            _category = catergory;
        }

        public void BuildBase(int x, int y)
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

        public void AddWeapon()
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

        public void AddArmor()
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

        public Tower GetResult()
        {
            return _tower;
        }
    }
}
