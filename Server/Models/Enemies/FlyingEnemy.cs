using TowerDefense.Enums;
using TowerDefense.Interfaces;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemy : Enemy, IPrototype<FlyingEnemy>
    {
        public override EnemyTypes Type => EnemyTypes.Flying;

        public FlyingEnemy(int x, int y) : base(x, y)
        {
            Flyweight = _flyweightFactory.GetFlyweight(GetEnemyFileName(EnemyTypes.Flying), 10);

            Health = 15;
            Speed = 3;
        }

        public FlyingEnemy ShallowClone()
        {
            var clonedEnemy = (FlyingEnemy)this.MemberwiseClone();

            return clonedEnemy;
        }
    }
}
