using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Interfaces.Visitor
{
    public interface IEffectVisitor
    {
        public EffectTypes EffectType { get; }

        public void Visit(Enemy enemy);

        public void Visit(MainObject mainObject);

        public void Visit(Tower tower);
    }
}
