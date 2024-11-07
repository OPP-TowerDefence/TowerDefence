using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemy : Enemy, IPrototype<StrongEnemy>
    {
        public override EnemyTypes Type => EnemyTypes.Strong;

        public StrongEnemy(int x, int y) : base(x, y)
        {
            Health = 30;
            Speed = 1;
        }

        public StrongEnemy Clone()
        {
            var clonedEnemy = (StrongEnemy)this.MemberwiseClone();
            // clonedEnemy.Id = Guid.NewGuid();
            // clonedEnemy._currentSpeedModifier = this._currentSpeedModifier;
            // clonedEnemy._modifierDuration = this._modifierDuration;
            // clonedEnemy._lastTilePosition = this._lastTilePosition;

            return clonedEnemy;
        }
    }
}
