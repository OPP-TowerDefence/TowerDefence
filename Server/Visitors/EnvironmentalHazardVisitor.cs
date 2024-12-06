using TowerDefense.Constants;
using TowerDefense.Enums;
using TowerDefense.Interfaces.Visitor;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Visitors
{
    public class EnvironmentalHazardVisitor(TowerTypes environmentType) : IEffectVisitor
    {
        public TowerTypes EnvironmentType { get; private set; } = environmentType;

        public EffectTypes EffectType => EffectTypes.EnvironmentalHazard;

        public void Visit(Enemy enemy)
        {
            enemy.Health -= Visitor.Environmental.EnemyDamage;
        }

        public void Visit(MainObject mainObject)
        {
            mainObject.DealDamage(Visitor.Environmental.MainObjectDamage);
        }

        public void Visit(Tower tower)
        {
            if (tower.Type == EnvironmentType)
            {
                tower.Weapon.IncreaseDamage(tower.Weapon.GetDamage() * Visitor.Environmental.TowerDamageChange);
            }
            else
            {
                tower.Weapon.IncreaseDamage(-(tower.Weapon.GetDamage() * Visitor.Environmental.TowerDamageChange));
            }
        }
    }
}
