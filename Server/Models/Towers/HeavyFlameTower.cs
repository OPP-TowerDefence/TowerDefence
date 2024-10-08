namespace TowerDefense.Models.Towers
{
    public class HeavyFlameTower : HeavyTower
    {
        public HeavyFlameTower(int x, int y) : base(x, y)
        {
            this.Power = 30;
            this.Range = 2;
            this.Speed = 3;
            this.Cost = 200;
        }
    }
}
