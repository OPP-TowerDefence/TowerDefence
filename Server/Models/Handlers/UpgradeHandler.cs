using TowerDefense.Enums;
using TowerDefense.Models.Towers;

namespace TowerDefense.Models.Handlers
{
    public abstract class UpgradeHandler
    {
        private UpgradeHandler _nextHandler;

        public void SetNext(UpgradeHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        protected void HandleNext(Tower tower, TowerUpgrades requestedUpgrade)
        {
            _nextHandler?.Handle(tower, requestedUpgrade);
        }

        public abstract void Handle(Tower tower, TowerUpgrades requestedUpgrade);
    }
}
