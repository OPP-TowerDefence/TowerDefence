﻿using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class HeavyFlameTower : HeavyTower
    {
        public override TowerTypes Type => TowerTypes.Flame;

        public HeavyFlameTower(int x, int y) : base(x, y)
        {
            this.Cost = 30;
            this.TicksToShoot = 9;
        }
    }
}
