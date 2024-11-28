using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Utils;

namespace TowerDefense.Models.Handlers
{
    public class ApplyUpgradeHandler : UpgradeHandler
    {
        public override void Handle(Tower tower, TowerUpgrades requestedUpgrade)
        {
            tower.Weapon = requestedUpgrade switch
            {
                TowerUpgrades.Burst => new Burst(tower.Weapon),
                TowerUpgrades.DoubleDamage => new DoubleDamage(tower.Weapon),
                TowerUpgrades.DoubleBullet => new DoubleBullet(tower.Weapon),
                _ => throw new Exception($"Unknown upgrade type: {requestedUpgrade}")
            };

            tower.AppliedUpgrades.Add(requestedUpgrade);

            Logger.Instance.LogInfo($"Upgrade {requestedUpgrade} successfully applied to tower at position ({tower.X},{tower.Y}).");
        }
    }
}
