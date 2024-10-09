﻿namespace TowerDefense.Models
{
    public abstract class Unit
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Unit(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
