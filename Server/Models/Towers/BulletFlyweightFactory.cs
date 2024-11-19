using static System.Net.Mime.MediaTypeNames;

namespace TowerDefense.Models.Towers
{
    public class BulletFlyweightFactory
    {
        private readonly Dictionary<string, BulletFlyweight> _flyweights = new();
        private readonly string _baseUrl;


        public BulletFlyweightFactory(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public BulletFlyweight GetFlyweight(string fileName, int speed)
        {
            var key = $"{fileName}-{speed}";

            if (!_flyweights.ContainsKey(key))
            {
                _flyweights[key] = new BulletFlyweight($"{_baseUrl}/{fileName}", speed);
            }
            return _flyweights[key];
        }
    }

}
