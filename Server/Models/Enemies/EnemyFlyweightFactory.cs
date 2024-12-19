namespace TowerDefense.Models.Enemies
{
    public class EnemyFlyweightFactory(string baseUrl)
    {
        private readonly Dictionary<string, EnemyFlyweight> _flyweights = [];
        private readonly string _baseUrl = baseUrl;

        public EnemyFlyweight GetFlyweight(string fileName, int rewardValue)
        {
            var key = $"{fileName}-{rewardValue}";

            if (!_flyweights.ContainsKey(key))
            {
                _flyweights[key] = new EnemyFlyweight($"{_baseUrl}/{fileName}", rewardValue);
            }

            return _flyweights[key];
        }
    }
}
