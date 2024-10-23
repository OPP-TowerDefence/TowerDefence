using TowerDefense.Enums;
namespace TowerDefense.Models.Towers
{
    public class HeavyLaserTower : HeavyTower
    {
        public override TowerTypes Type => TowerTypes.Laser;
        public HeavyLaserTower(int x, int y) : base(x, y)
        {
            this.Cost = 200;
            this.TicksToShoot = 8;
        }
    }
}
