using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class LongDistanceFlameTower : LongDistanceTower
    {
        public override TowerTypes Type => TowerTypes.Flame;
        public LongDistanceFlameTower(int x, int y) : base(x, y)
        {
            this.Power = 3;
            this.Range = 10;
            this.Speed = 7;
            this.Cost = 100;
        }
    }
}
