namespace TowerDefense.Models.Towers
{
    public class Armor
    {
        public string Name { get; set; }
        public int Defense { get; set; }

        public Armor(string name, int defense)
        {
            Name = name;
            Defense = defense;
        }
    }
}
