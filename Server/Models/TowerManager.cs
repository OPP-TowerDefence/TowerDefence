using System.Resources;
using TowerDefense.Enums;
using TowerDefense.Models.Commands;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models
{
    public class TowerManager
    {
        public List<Tower> Towers { get; set; } = new List<Tower>();

        public bool IsOccupied(int x, int y)
        {
            return Towers.Any(t => t.X == x && t.Y == y);
        }

        public void UpgradeTower(int x, int y, TowerUpgrades towerUpgrade, Utils.ResourceManager resourceManager, LevelProgressionFacade levelFacade)
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

        public Tower BuyTower(int x, int y, TowerCategories towerCategory, Player player)
        {
            var tower = player.CreateTower(x, y, towerCategory);

            return tower;
        }
    }
}
