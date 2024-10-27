namespace TowerDefense.Models
{
    public class MainObject : Unit
    {
        private const int _maxHealth = 100;
        public int Health { get; private set; } = _maxHealth;

        public MainObject(int x, int y) : base(x, y)
        {
        }

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
