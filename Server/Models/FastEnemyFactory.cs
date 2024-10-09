﻿namespace TowerDefense.Models
{
    public class FastEnemyFactory : IEnemyFactory
    {
        public Enemy CreateEnemy()
        {
            return new FastEnemy { X = 0, Y = 0 };
        }
    }
}
