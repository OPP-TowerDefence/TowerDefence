using System.Xml.Serialization;
using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class FlameTowerBuilder : ITowerBuilder
    {
        private Tower _tower;
        private TowerCategories _category;

        public FlameTowerBuilder(TowerCategories catergory)
        {
            _category = catergory;
        }

        public void BuildBase(int x, int y)
        {
            if(_category == TowerCategories.Heavy)
            {
                _tower = new HeavyFlameTower(x, y);
            }
            else
            {
                _tower = new LongDistanceFlameTower(x, y);
            }
        }

        public void AddWeapon()
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

        public void AddArmor()
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

        public Tower GetResult()
        {
            return _tower;
        }
    }
}
