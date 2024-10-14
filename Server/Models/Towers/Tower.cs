using TowerDefense.Enums;

namespace TowerDefense.Models.Towers;
public abstract class Tower : Unit
{
    public int Cost { get; set; }
    public Weapon Weapon { get; set; }
    public Armor Armor { get; set; }
    public abstract TowerTypes Type { get; }
    public abstract TowerCategories Category { get; }

    public Tower(int x, int y): base(x, y)
    {
    }
}
