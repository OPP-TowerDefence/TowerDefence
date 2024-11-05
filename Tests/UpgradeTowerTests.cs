using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Enums;
using TowerDefense.Models.Towers;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class UpgradeTowerTests : GameStateTests
    {
        [TestMethod]
        public void UpgradeTower_ApplyUpgrade_WhenUpgradeIsValid()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            _gameState.Map.Towers.Add(tower);

            // Act
            _gameState.UpgradeTower(0, 0, TowerUpgrades.DoubleDamage);

            // Assert
            Assert.IsTrue(tower.AppliedUpgrades.Contains(TowerUpgrades.DoubleDamage), "Upgrade should be applied to the tower.");
        }

        [TestMethod]
        public void UpgradeTower_DoesNotApplyDuplicateUpgrade_WhenTowerAlreadyHasUpgrade()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            _gameState.Map.Towers.Add(tower);

            // Act
            _gameState.UpgradeTower(0, 0, TowerUpgrades.DoubleDamage);
            _gameState.UpgradeTower(0, 0, TowerUpgrades.DoubleDamage);

            // Assert
            Assert.AreEqual(1, tower.AppliedUpgrades.Count(u => u == TowerUpgrades.DoubleDamage), "Duplicate upgrade should not be applied.");
        }

        [TestMethod]
        public void UpgradeTower_AppliesMultipleUpgrades_WhenEachUpgradeIsDifferent()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            _gameState.Map.Towers.Add(tower);

            // Act
            _gameState.UpgradeTower(0, 0, TowerUpgrades.DoubleDamage);
            _gameState.UpgradeTower(0, 0, TowerUpgrades.Burst);
            _gameState.UpgradeTower(0, 0, TowerUpgrades.DoubleBullet);

            // Assert
            Assert.IsTrue(tower.AppliedUpgrades.Contains(TowerUpgrades.DoubleDamage), "DoubleDamage upgrade should be applied to the tower.");
            Assert.IsTrue(tower.AppliedUpgrades.Contains(TowerUpgrades.Burst), "Burst upgrade should be applied to the tower.");
            Assert.IsTrue(tower.AppliedUpgrades.Contains(TowerUpgrades.DoubleBullet), "Double bullet upgrade should be applied to the tower.");
        }

        [TestMethod]
        public void UpgradeTower_DoesNotApplyUpgrade_WhenTowerNotFound()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            _gameState.Map.Towers.Add(tower);

            // Act
            _gameState.UpgradeTower(1, 1, TowerUpgrades.DoubleDamage);

            // Assert
            Assert.IsFalse(tower.AppliedUpgrades.Contains(TowerUpgrades.DoubleDamage), "Upgrade should not be applied to a non-existent tower.");
        }

        [TestMethod]
        public void UpgradeTower_ThrowsException_WhenUpgradeIsInvalid()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            _gameState.Map.Towers.Add(tower);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => _gameState.UpgradeTower(0, 0, (TowerUpgrades)999), "Exception should be thrown for an invalid upgrade type.");
        }
    }
}
