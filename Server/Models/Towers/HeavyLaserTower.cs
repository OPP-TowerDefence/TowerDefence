namespace TowerDefense.Models.Towers
{
    public class HeavyLaserTower : HeavyTower
    {
        public HeavyLaserTower(int x, int y) : base(x, y)
        {
            this.Power = 15;
            this.Range = 5;
            this.Speed = 5;
            this.Cost = 200;
        }
    }
}
