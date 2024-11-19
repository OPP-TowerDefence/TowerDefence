using static System.Net.Mime.MediaTypeNames;

namespace TowerDefense.Models.Towers
{
    public class BulletFlyweight
    {
        public string FileName { get; private set; }
        public string Path { get; private set; }
        public int Speed { get; private set; }

        public BulletFlyweight(string fileName, string baseUrl, int speed)
        {
            FileName = fileName;
            Path = $"{baseUrl}/{fileName}";
            Speed = speed;
        }
    }
}
