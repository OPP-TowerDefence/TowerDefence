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
        public FastEnemy ShallowClone()
        {
            return (FastEnemy)this.MemberwiseClone();
        }
        public FastEnemy DeepClone()
        {
            var deepClonedEnemy = new FastEnemy(this.X, this.Y)
            {
                Health = this.Health,
                Speed = this.Speed,
                RewardValue = this.RewardValue,
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
