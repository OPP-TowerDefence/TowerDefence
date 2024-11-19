namespace TowerDefense.Models.Towers
{
    public class BulletFlyweight(string path, int speed)
    {
        public string Path { get; private set; } = path;
        public int Speed { get; private set; } = speed;
    }
}
