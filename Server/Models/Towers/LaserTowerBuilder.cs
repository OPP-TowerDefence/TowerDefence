﻿using System.Xml.Serialization;
using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Towers
{
    public class LaserTowerBuilder : TowerBuilder
    {

        public LaserTowerBuilder(TowerCategories catergory) : base(catergory)
        {
        }

        public override void BuildBase(int x, int y)
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

        public override void AddWeapon()
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

        public override void AddArmor()
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

        public override Tower GetResult()
        {
            return _tower;
        }
    }
}
