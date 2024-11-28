using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models.Handlers
{
    public class ResourceCheckHandler(ResourceManager recourceManager) : UpgradeHandler
    {
        private const int _upgradeCostForBurst = 30;
        private const int _upgradeCostForDoubleBullet = 60;
        private const int _upgradeCostForDoubleDamage = 90;

        private readonly ResourceManager _resourceManager = recourceManager;

        public override void Handle(Tower tower, TowerUpgrades requestedUpgrade)
        {
            int upgradeCost = requestedUpgrade switch
            {
                TowerUpgrades.Burst => _upgradeCostForBurst,
                TowerUpgrades.DoubleBullet => _upgradeCostForDoubleBullet,
                TowerUpgrades.DoubleDamage => _upgradeCostForDoubleDamage,
                _ => throw new Exception($"Unknown upgrade type: {requestedUpgrade}")
            };

            if (!_resourceManager.DeductResources(upgradeCost))
            {
                return;
            }

            HandleNext(tower, requestedUpgrade);
        }
    }
}
