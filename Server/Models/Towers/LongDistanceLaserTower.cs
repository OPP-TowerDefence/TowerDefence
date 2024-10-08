namespace TowerDefense.Models.Towers
{
    public class LongDistanceLaserTower : LongDistanceTower
    {
        public LongDistanceLaserTower(int x, int y) : base(x, y)
        {
            this.Power = 1;
            this.Range = 10;
            this.Speed = 10;
            this.Cost = 100;
        }
    }
}
