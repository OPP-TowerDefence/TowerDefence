﻿using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models.Handlers
{
    public class LevelRequirementHandler(LevelProgressionFacade levelProgressionFacade) : UpgradeHandler
    {
        private const int _requiredLevelForBurst = 1;
        private const int _requiredLevelForDoubleBullet = 2;
        private const int _requiredLevelForDoubleDamage = 3;

        private readonly LevelProgressionFacade _levelProgressionFacade = levelProgressionFacade;

        public override void Handle(Tower tower, TowerUpgrades requestedUpgrade)
        {
            int requiredLevel = requestedUpgrade switch
            {
                TowerUpgrades.Burst => _requiredLevelForBurst,
                TowerUpgrades.DoubleBullet => _requiredLevelForDoubleBullet,
                TowerUpgrades.DoubleDamage => _requiredLevelForDoubleDamage,
                _ => throw new Exception($"Unknown upgrade type: {requestedUpgrade}")
            };

            int currentLevel = _levelProgressionFacade.GetCurrentLevel();

            if (currentLevel < requiredLevel)
            {
                return;
            }

            HandleNext(tower, requestedUpgrade);
        }
    }
}
