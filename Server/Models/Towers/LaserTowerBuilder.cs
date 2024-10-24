using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class LaserTowerBuilder(TowerCategories catergory) : TowerBuilder(catergory)
    {
        public override LaserTowerBuilder AddArmor()
        {

            if (_category == TowerCategories.Heavy)
            {
                _tower.Armor = new Armor("Laser Shield", 15);
            }
            else
            {
                _tower.Armor = new Armor("Laser Shield", 7);
            }

            return this;
        }

        public override LaserTowerBuilder AddWeapon()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Weapon = new Weapon("Laser Cannon", 15, 5, 4);
            }
            else
            {
                _tower.Weapon = new Weapon("Laser Blaster", 1, 10, 10);
            }

            return this;
        }

        public override LaserTowerBuilder BuildBase(int x, int y)
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower = new HeavyLaserTower(x, y);
            }
            else
            {
                _tower = new LongDistanceLaserTower(x, y);
            }

            return this;
        }

        public override Tower GetResult()
        {
            return _tower;
        }
    }
}
