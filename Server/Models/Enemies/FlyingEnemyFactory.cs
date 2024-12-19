using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class FlyingEnemyFactory : IEnemyFactory
    {
        private const string _fileName = "flyingEnemy.gif";
        private const int _rewardValue = 10;

        private readonly FlyingEnemy _prototypeEnemy;

        public FlyingEnemyFactory()
        {
            _prototypeEnemy = new FlyingEnemy(0, 0, GameState.FlyweightFactory.GetFlyweight(_fileName, _rewardValue));
            _prototypeEnemy.SetInitialStrategy(new SpeedPrioritizationStrategy());
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
