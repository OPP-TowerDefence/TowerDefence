using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemy : Enemy, IPrototype<StrongEnemy>
    {
        public override EnemyTypes Type => EnemyTypes.Strong;

        public StrongEnemy(int x, int y) : base(x, y)
        {
            Flyweight = _flyweightFactory.GetFlyweight(GetEnemyFileName(EnemyTypes.Strong), 15);

            Health = 20;
            Speed = 2;
        }

        public StrongEnemy ShallowClone()
        {
            var clonedEnemy = (StrongEnemy)this.MemberwiseClone();

            return clonedEnemy;
        }
    }
}
