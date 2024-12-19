using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class StrongEnemyFactory : IEnemyFactory
    {
        private const string _fileName = "strongEnemy.gif";
        private const int _rewardValue = 15;

        private readonly StrongEnemy _prototypeEnemy;

        public StrongEnemyFactory()
        {
            _prototypeEnemy = new StrongEnemy(0, 0, GameState.FlyweightFactory.GetFlyweight(_fileName, _rewardValue));
            _prototypeEnemy.SetInitialStrategy(new SurvivalStrategy());
        }

        public Enemy CreateEnemy(int x, int y)
        {
            var enemy = _prototypeEnemy.ShallowClone();
            enemy.X = x;
            enemy.Y = y;
            return enemy;
        }
    }
}
