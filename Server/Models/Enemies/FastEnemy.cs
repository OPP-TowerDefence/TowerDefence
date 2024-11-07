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
        public FastEnemy Clone()
        {
            return (FastEnemy)this.MemberwiseClone();
        }
    }
}
