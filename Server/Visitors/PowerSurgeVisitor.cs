﻿using TowerDefense.Constants;
using TowerDefense.Enums;
using TowerDefense.Interfaces.Visitor;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;

namespace TowerDefense.Visitors
{
    public class PowerSurgeVisitor : IEffectVisitor
    {
        public EffectTypes EffectType => EffectTypes.PowerSurge;

        public void Visit(Enemy enemy)
        {
            enemy.IncreaseSpeed(Visitor.PowerSurge.EnemySpeedDecrease);
        }

        public void Visit(MainObject mainObject)
        {
            mainObject.Repair(Visitor.PowerSurge.MainObjectRepair);
        }

        public void Visit(Tower tower)
        {
            tower.Weapon.SetDamage(tower.Weapon.GetDamage() * Visitor.PowerSurge.TowerDamageIncrease);

            tower.Weapon.SetRange(tower.Weapon.GetRange() * Visitor.PowerSurge.TowerRangeIncrease);

            tower.Weapon.IncreaseSpeed(Visitor.PowerSurge.TowerSpeedIncrease);
        }
    }
}
