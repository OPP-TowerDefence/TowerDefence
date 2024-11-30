using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models.Handlers
{
    public class UpgradeAvailabilityHandler : UpgradeHandler
    {
        public override void Handle(Tower tower, TowerUpgrades requestedUpgrade)
        {
            if (tower.AppliedUpgrades.Contains(requestedUpgrade))
            {
                return;
            }

            HandleNext(tower, requestedUpgrade);
        }
    }
}
