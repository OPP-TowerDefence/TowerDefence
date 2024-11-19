using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public class LongDistanceFlameTower : LongDistanceTower
    {
        public override TowerTypes Type => TowerTypes.Flame;
        public override string BulletFileName => "fireBullet.gif";

        public LongDistanceFlameTower(int x, int y) : base(x, y)
        {
            this.Cost = 100;
            this.TicksToShoot = 4;
        }
    }
}
