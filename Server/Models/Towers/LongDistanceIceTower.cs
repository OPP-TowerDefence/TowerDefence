using TowerDefense.Enums;
namespace TowerDefense.Models.Towers
{
    public class LongDistanceIceTower : LongDistanceTower
    {
        public override TowerTypes Type => TowerTypes.Ice;
        public LongDistanceIceTower(int x, int y) : base(x, y)
        {
            this.Power = 5;
            this.Range = 10;
            this.Speed = 5;
            this.Cost = 100;
        }
    }
}
