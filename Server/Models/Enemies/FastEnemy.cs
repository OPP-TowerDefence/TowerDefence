using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class FastEnemy : Enemy, IPrototype<FastEnemy>
    {
        public override EnemyTypes Type => EnemyTypes.Fast;

        public FastEnemy(int x, int y, EnemyFlyweight enemyFlyweight) : base(x, y, enemyFlyweight)
        {
            Health = 10;
            Speed = 4;
        }

        public FastEnemy ShallowClone()
        {
            return (FastEnemy)this.MemberwiseClone();
        }

        public FastEnemy DeepClone()
        {
            var deepClonedEnemy = new FastEnemy(this.X, this.Y, new EnemyFlyweight(this.FileName, this.RewardValue))
            {
                Health = this.Health,
                Speed = this.Speed,
                Id = Guid.NewGuid(),
                _currentSpeedModifier = this._currentSpeedModifier,
                _modifierDuration = this._modifierDuration,
                _lastTilePosition = (this._lastTilePosition.x, this._lastTilePosition.y),
            };

            foreach (var effect in _scheduledEffects)
            {
                deepClonedEnemy._scheduledEffects.Add((effect.effect, effect.turnsRemaining));
            }

            if (this.CurrentStrategy != null)
            {
                deepClonedEnemy.SetInitialStrategy(this.CurrentStrategy);
            }

            return deepClonedEnemy;
        }
    }
}
