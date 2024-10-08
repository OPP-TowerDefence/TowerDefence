namespace TowerDefense.Models.Towers;
public abstract class Tower : Unit
{
    public int Range { get; set; }
    public int Power { get; set; }
    public int Speed { get; set; }
    public int Cost { get; set; }

    public Tower(int x, int y): base(x, y)
    {
    }
}
