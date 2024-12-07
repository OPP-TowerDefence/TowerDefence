﻿using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemy : Enemy, IPrototype<StrongEnemy>
    {
        public override EnemyTypes Type => EnemyTypes.Strong;

        public StrongEnemy(int x, int y) : base(x, y)
        {
            Health = 20;
            Speed = 2;
        }

        public StrongEnemy ShallowClone()
        {
            var clonedEnemy = (StrongEnemy)this.MemberwiseClone();

            return clonedEnemy;
        }
    }
}
