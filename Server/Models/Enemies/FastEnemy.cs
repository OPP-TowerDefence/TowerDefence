using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class FastEnemy : Enemy, IPrototype<FastEnemy>
    {
        public override EnemyTypes Type => EnemyTypes.Fast;

        public FastEnemy(int x, int y) : base(x, y)
        {
            Health = 20;
            Speed = 3;
        }

        public FastEnemy Clone()
        {
            var clonedEnemy = (FastEnemy)this.MemberwiseClone();

            
            //deep copy
            // clonedEnemy.Id = Guid.NewGuid();
            // clonedEnemy._currentSpeedModifier = this._currentSpeedModifier;
            // clonedEnemy._modifierDuration = this._modifierDuration;
            // clonedEnemy._lastTilePosition = this._lastTilePosition;

            return clonedEnemy;
        }
    }
}
