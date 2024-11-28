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
                Logger.Instance.LogError($"Tower at position ({tower.X},{tower.Y}) already has the {requestedUpgrade} upgrade.");
                return;
            }

            Logger.Instance.LogInfo($"Upgrade availability check passed for {requestedUpgrade}.");
            HandleNext(tower, requestedUpgrade);
        }
    }
}
