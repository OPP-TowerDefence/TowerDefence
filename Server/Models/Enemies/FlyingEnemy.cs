using TowerDefense.Enums;
using TowerDefense.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemy : Enemy, IPrototype<FlyingEnemy>
    {
        public override EnemyTypes Type => EnemyTypes.Flying;

        public FlyingEnemy(int x, int y) : base(x, y)
        {
            Health = 25;
            Speed = 2;
        }
        public FlyingEnemy Clone()
        {
            var clonedEnemy = (FlyingEnemy)this.MemberwiseClone();
            // clonedEnemy.Id = Guid.NewGuid();
            // clonedEnemy._currentSpeedModifier = this._currentSpeedModifier;
            // clonedEnemy._modifierDuration = this._modifierDuration;
            // clonedEnemy._lastTilePosition = this._lastTilePosition;

            return clonedEnemy;
        }
    }
}
