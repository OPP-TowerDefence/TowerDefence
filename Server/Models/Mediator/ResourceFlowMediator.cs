using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models.Mediator
{
    public class ResourceFlowMediator(TowerManager towerManager, AchievementManager achievementManager, ResourceManager resourceManager, GameState gameState) :IMediator
    {
        private TowerManager _towerManager = towerManager;
        private AchievementManager _achievementManager = achievementManager;
        private ResourceManager _resourceManager = resourceManager;
        private readonly GameState _gameState = gameState;
    
        public void Notify(object sender, string eventCode, object data)
        {
            if (Enum.TryParse(eventCode, out AchievementMediatorEvents achievementEvent) == false)
            {
                Logger.Instance.LogError($"Invalid event code: {eventCode}");
                return;
            }

            switch (achievementEvent)
            {
                case AchievementMediatorEvents.TowerPlaced:
                    var tower = (Tower)data;
                    _achievementManager.AddTowersPlaced(1);
                    _resourceManager.OnTowerPurchase(tower.Cost);
                    break;
                case AchievementMediatorEvents.EnemyKilled:
                    _achievementManager.AddEnemiesKilled((int)data);
                    break;
                case AchievementMediatorEvents.ResourceSpent:
                    _achievementManager.TrackResourceGained((int)data);
                    break;
                case AchievementMediatorEvents.AchievementUnlocked:
                    var d = (Achievement)data;
                    _resourceManager.OnAchievementReached(d.Reward);
                    _gameState.DisplayMessage(d.Message);
                    break;
                default: return;
            }
        }
    }
}
