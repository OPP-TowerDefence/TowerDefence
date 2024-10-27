namespace TowerDefense.Models
{
    public class MainObject(int x, int y)
    {
        private const int _maxHealth = 100;
        public int Health { get; private set; } = _maxHealth;

        public int X { get; private set; } = x;
        public int Y { get; private set; } = y;

        public void DecreaseHealth(int amount)
        {
            Health = Math.Max(0, Health - amount);
        }

        public void IncreaseHealth(int amount)
        {
            Health = Math.Min(Health + amount, _maxHealth);
        }
    }
}
