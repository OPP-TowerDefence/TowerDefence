using System.Xml.Serialization;
using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class LaserTowerBuilder : ITowerBuilder
    {
        private Tower _tower;
        private TowerCategories _category;

        public LaserTowerBuilder(TowerCategories catergory)
        {
            _category = catergory;
        }

        public void BuildBase(int x, int y)
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower = new HeavyLaserTower(x, y);
            }
            else
            {
                _tower = new LongDistanceLaserTower(x, y);
            }
        }

        public void AddWeapon()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Weapon = new Weapon("Laser Cannon", 15, 5, 5);
            }
            else
            {
                _tower.Weapon = new Weapon("Laser Blaster", 1, 10, 10);
            }
        }

        public void AddArmor()
        {

            if (_category == TowerCategories.Heavy)
            {
                _tower.Armor = new Armor("Laser Shield", 15);
            }
            else
            {
                _tower.Armor = new Armor("Laser Shield", 7);
            }
        }

        public Tower GetResult()
        {
            return _tower;
        }
    }
}
