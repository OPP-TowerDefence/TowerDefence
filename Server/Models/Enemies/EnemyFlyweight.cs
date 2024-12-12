namespace TowerDefense.Models.Enemies
{
    public class EnemyFlyweight(string fileName, int rewardValue)
    {
        public string FileName { get; set; } = fileName;

        public int RewardValue { get; private set; }
    }
}
