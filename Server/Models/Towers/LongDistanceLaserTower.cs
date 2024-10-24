﻿using TowerDefense.Enums;
namespace TowerDefense.Models.Towers
{
    public class LongDistanceLaserTower : LongDistanceTower
    {
        public override TowerTypes Type => TowerTypes.Laser;
        public LongDistanceLaserTower(int x, int y) : base(x, y)
        {
            this.Cost = 100;
            this.TicksToShoot = 4;
        }
    }
}
