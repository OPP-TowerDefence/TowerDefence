namespace TowerDefense.Models.Towers
{
    public class BulletFlyweightFactory(string baseUrl)
    {
        private readonly Dictionary<string, BulletFlyweight> _flyweights = new();
        private readonly string _baseUrl = baseUrl;

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
