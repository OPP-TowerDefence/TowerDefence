using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class IceTowerBuilder(TowerCategories catergory) : TowerBuilder(catergory)
    {
        public override IceTowerBuilder AddArmor()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Armor = new Armor("Ice Barrier", 20);
            }
            else
            {
                _tower.Armor = new Armor("Ice Barrier", 10);
            }

            return this;
        }

        public override IceTowerBuilder AddWeapon()
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower.Weapon = new Weapon("Ice Cannon", 8, 25, 2);
            }
            else
            {
                _tower.Weapon = new Weapon("Ice Gun", 4, 40, 4);
            }

            return this;
        }

        public override IceTowerBuilder BuildBase(int x, int y)
        {
            if (_category == TowerCategories.Heavy)
            {
                _tower = new HeavyIceTower(x, y);
            }
            else
            {
                _tower = new LongDistanceIceTower(x, y);
            }

            return this;
        }

        public override Tower GetResult()
        {
            return _tower;
        }
    }
}
