﻿using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class LongDistanceFlameTower : LongDistanceTower
    {
        public override TowerTypes Type => TowerTypes.Flame;

        public LongDistanceFlameTower(int x, int y) : base(x, y)
        {
            this.Cost = 40;
            this.TicksToShoot = 4;
        }
    }
}
