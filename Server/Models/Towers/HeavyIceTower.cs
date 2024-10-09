﻿using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class HeavyIceTower : HeavyTower
    {
        public override TowerTypes Type => TowerTypes.Ice;
        public HeavyIceTower(int x, int y) : base(x, y)
        {
            this.Power = 40;
            this.Range = 2;
            this.Speed = 1;
            this.Cost = 200;
        }
    }
}
