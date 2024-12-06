using TowerDefense.Constants;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Visitors;

namespace TowerDefense.Utils
{
    public static class EffectResolver
    {
        private static readonly Random _random = new();

        public static IEnumerable<Effect> GetEffects(int level)
        {
            var effects = new List<Effect>();

            if (level % level == 0)
            {
                effects.Add(new Effect
                {
                    Applicator = new PowerSurgeVisitor(),
                    DurationInTicks = Visitor.PowerSurge.DurationInTicks,
                    Reverser = new PowerSurgeReversalVisitor()
                });
            }

            if (_random.NextDouble() < 0.99)
            {
                var effectType = _random.Next(0, 3) switch
                {
                    0 => TowerTypes.Ice,
                    1 => TowerTypes.Flame,
                    _ => TowerTypes.Laser
                };

                effects.Add(new Effect
                {
                    Applicator = new EnvironmentalHazardVisitor(effectType),
                    DurationInTicks = Visitor.Environmental.DurationInTicks,
                    Reverser = new EnvironmentalHazardReversalVisitor(effectType)
                });
            }

            return effects;
        }
    }
}
