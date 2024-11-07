namespace TowerDefense.Models
{
    public class Armor(string name, int defense)
    {
        public string Name { get; set; } = name;
        public int Defense { get; set; } = defense;
    }
}
