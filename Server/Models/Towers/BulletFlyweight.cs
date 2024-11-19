using static System.Net.Mime.MediaTypeNames;

namespace TowerDefense.Models.Towers
{
    public class BulletFlyweight
    {
        public string Path { get; private set; }
        public int Speed { get; private set; }

        public BulletFlyweight(string path, int speed)
        {
            Path = path;
            Speed = speed;
        }
    }
}
