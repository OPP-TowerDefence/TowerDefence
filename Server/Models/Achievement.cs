namespace TowerDefense.Models
{
    public class Achievement(string message, int reward)
    {
        public string Message { get; set; } = message;
        public int Reward { get; set; } = reward;
    }
}
