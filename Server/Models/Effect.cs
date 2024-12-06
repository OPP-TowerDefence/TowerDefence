using TowerDefense.Interfaces.Visitor;

namespace TowerDefense.Models
{
    public class Effect
    {
        public required IEffectVisitor Applicator { get; set; }

        public required IEffectVisitor Reverser { get; set; }

        public required int DurationInTicks { get; set; }
    }
}
