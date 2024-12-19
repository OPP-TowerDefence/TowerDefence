using TowerDefense.Interfaces;
using TowerDefense.Models.Strategies;

namespace TowerDefense.Models.Enemies
{
    public class FastEnemyFactory : IEnemyFactory
    {
        private const string _fileName = "fastEnemy.gif";
        private const int rewardValue = 10;
        
        private readonly FastEnemy _prototypeEnemy;

        public FastEnemyFactory()
        {
            _prototypeEnemy = new FastEnemy(0, 0, GameState.FlyweightFactory.GetFlyweight(_fileName, rewardValue));
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
