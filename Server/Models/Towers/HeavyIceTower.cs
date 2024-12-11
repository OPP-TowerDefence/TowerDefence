using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class HeavyIceTower : HeavyTower
    {
        public override TowerTypes Type => TowerTypes.Ice;

        public HeavyIceTower(int x, int y) : base(x, y)
        {
            this.Cost = 25;
            this.TicksToShoot = 10;
        }
    }
}
