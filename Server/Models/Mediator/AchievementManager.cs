using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Utils;

namespace TowerDefense.Models.Mediator
{
    public class AchievementManager
    {

        private IMediator? _mediator;
        private int resourcesSpent;
        private int enemiesKilled;
        private int towersPlaced;
        private int resourceSpentReward;
        private int enemyKilledReward;
        private int towerPlacedReward;
        private int resourceSpentGoal;
        private int enemyKilledGoal;
        private int towerPlacedGoal;

        public AchievementManager(IMediator? mediator)
        {
            _mediator = mediator;

            resourcesSpent = 0;
            enemiesKilled = 0;
            towersPlaced = 0;

            resourceSpentReward = 20;
            enemyKilledReward = 100;
            towerPlacedReward = 100;

            resourceSpentGoal = 200;
            enemyKilledGoal = 5;
            towerPlacedGoal = 5;
        }

        public void SetMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void TrackResourceGained(int resource)
        {
            resourcesSpent += resource;

            if(resourcesSpent >= resourceSpentGoal)
            {
                resourceSpentGoal *= 2;
                resourcesSpent = 0;

                AchievementReached($"Resource Hoarder! Gained {resourceSpentReward} resources. Spend {resourceSpentGoal} more Resources for the next Reward", resourceSpentReward);

                resourceSpentReward *= 2;
            }
        }

        public void AddEnemiesKilled(int amount)
        {
            enemiesKilled += amount;

            if(enemiesKilled >= enemyKilledGoal)
            {
                enemyKilledGoal *= 3;
                enemiesKilled = 0;

                AchievementReached($"Killing Spree! Gained {enemyKilledReward} resources. Kill {enemyKilledGoal} more Enemies for the next Reward", enemyKilledReward);

                enemyKilledReward *= 2;
            }
        }

        public void AddTowersPlaced(int amount)
        {
            towersPlaced += amount;

            if(towersPlaced >= towerPlacedGoal)
            {
                towerPlacedGoal *= 2;
                towersPlaced = 0;

                AchievementReached($"Tower Builder! Gained {towerPlacedReward} resources. Place {towerPlacedGoal} more Towers for the next Reward", towerPlacedReward);

                towerPlacedReward *= 2;
            }
        }

        private void AchievementReached(string message, int reward)
        {
            if(_mediator == null)
            {
                Logger.Instance.LogError("Mediator is not set in AchievementManager");
                return;
            }

            var achievement = new Achievement(message, reward);
            _mediator.Notify(this, AchievementMediatorEvents.AchievementUnlocked.ToString(), achievement);
        }
    }
}
