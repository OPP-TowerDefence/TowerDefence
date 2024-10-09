using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class HeavyFlameTower : HeavyTower
    {
        public override TowerTypes Type => TowerTypes.Flame;
        public HeavyFlameTower(int x, int y) : base(x, y)
        {
            this.Power = 30;
            this.Range = 2;
            this.Speed = 3;
            this.Cost = 200;
        }
    }
}
