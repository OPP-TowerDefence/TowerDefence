using TowerDefense.Enums;

namespace TowerDefense.Models.Towers;
public abstract class Tower : Unit
{
    public int Range { get; set; }
    public int Power { get; set; }
    public int Speed { get; set; }
    public int Cost { get; set; }
    public abstract TowerTypes Type { get; }
    public abstract TowerCategories Category { get; }

    public Tower(int x, int y): base(x, y)
    {
    }
}
