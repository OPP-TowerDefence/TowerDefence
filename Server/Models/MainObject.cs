namespace TowerDefense.Models
{
    public class MainObject(int x, int y, int initialHealth = MainObject._maxHealth) : Unit(x, y)
    {
        private const int _maxHealth = 100;

        public int Health { get; private set; } = initialHealth;

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
