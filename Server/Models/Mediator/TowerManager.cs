using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models.Mediator
{
    public class TowerManager
    {
        private IMediator? _mediator;

        public TowerManager(IMediator? mediator)
        {
            _mediator = mediator;
        }
        public void SetMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public List<Tower> Towers { get; set; } = new List<Tower>();

        public bool IsOccupied(int x, int y)
        {
            return Towers.Any(t => t.X == x && t.Y == y);
        }

        public void UpgradeTower(int x, int y, TowerUpgrades towerUpgrade, ResourceManager resourceManager, LevelProgressionFacade levelFacade)
        {
            var tower = Towers.FirstOrDefault(t => t.X == x && t.Y == y);

            if (tower is null)
            {
                Logger.Instance.LogError($"Unable to upgrade tower at position ({x},{y}). Tower not found.");

                return;
            }

            var upgradeProcessor = new UpgradeProcessor(resourceManager, levelFacade);
            upgradeProcessor.Process(tower, towerUpgrade);
        }

        public Tower? BuyTower(int x, int y, TowerCategories towerCategory, Player player)
        {
            if (_mediator == null)
            {
                Logger.Instance.LogError("Mediator is not set in TowerManager");
                return null;
            }

            var tower = player.CreateTower(x, y, towerCategory);

            _mediator.Notify(this, AchievementMediatorEvents.TowerPlaced.ToString(), tower);
            return tower;
        }

        public void SetTowers(List<Tower> towers)
        {
            Towers = towers;
        }
    }
}
