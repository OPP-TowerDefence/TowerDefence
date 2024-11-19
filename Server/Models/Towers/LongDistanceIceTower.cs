using TowerDefense.Enums;
namespace TowerDefense.Models.Towers
{
    public class LongDistanceIceTower : LongDistanceTower
    {
        public override TowerTypes Type => TowerTypes.Ice;

        public LongDistanceIceTower(int x, int y) : base(x, y)
        {
            this.Cost = 100;
            this.TicksToShoot = 4;
        }
    }
}
