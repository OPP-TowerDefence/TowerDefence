using TowerDefense.Enums;
using TowerDefense.Models.Handlers;
using TowerDefense.Models.Mediator;
using TowerDefense.Models.Towers;

namespace TowerDefense.Utils
{
    public class UpgradeProcessor
    {
        private readonly UpgradeHandler _chain;

        public UpgradeProcessor(ResourceManager resourceManager, LevelProgressionFacade levelProgressionFacade)
        {
            var upgradeAvailability = new UpgradeAvailabilityHandler();
            var levelRequirement = new LevelRequirementHandler(levelProgressionFacade);
            var resourceCheck = new ResourceCheckHandler(resourceManager);
            var applyUpgrade = new ApplyUpgradeHandler();

            upgradeAvailability.SetNext(levelRequirement);
            levelRequirement.SetNext(resourceCheck);
            resourceCheck.SetNext(applyUpgrade);

            _chain = upgradeAvailability;
        }

        public void Process(Tower tower, TowerUpgrades requestedUpgrade)
        {
            _chain.Handle(tower, requestedUpgrade);
        }
    }
}
