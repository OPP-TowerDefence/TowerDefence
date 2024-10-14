namespace TowerDefense.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Range { get; set; }
        public int Speed { get; set; }

        public Weapon(string name, int damage, int range, int speed)
        {
            Name = name;
            Damage = damage;
            Range = range;
            Speed = speed;
        }
    }
}
