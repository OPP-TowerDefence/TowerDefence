using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Models;
using TowerDefense.Tests;
using TowerDefense.Models.Towers;
using TowerDefense.Enums;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class GetMapTowersTests : GameStateTests
    {
        [TestMethod]
        public void GetMapTowers_ReturnsEmptyList_WhenNoTowers()
        {
            // Arrange
            _gameState.Map.Towers.Clear();
            var expectedTowersCount = 0;

            // Act
            var result = _gameState.GetMapTowers() as IList;

            // Assert
            Assert.AreEqual(expectedTowersCount, result.Count, "GetMapTowers should return an empty list when no towers are present");
        }

        [TestMethod]
        public void GetMapTowers_ReturnsMapTowers_ThereAreTowersWithoutUpgrades()
        {
            // Arrange
            _gameState.Map.Towers.Add(new HeavyLaserTower(1, 1));
            _gameState.Map.Towers.Add(new HeavyLaserTower(1, 1));
            _gameState.Map.Towers.Add(new HeavyLaserTower(1, 1));

            var expectedTowersCount = _gameState.Map.Towers.Count;

            // Act
            var result = _gameState.GetMapTowers() as IList;

            // Assert
            Assert.AreEqual(expectedTowersCount, result.Count, "GetMapTowers should return all towers");
        }

        [TestMethod]
        public void GetMapTowers_ReturnsMapTowers_ThereAreTowersWithUpgrades()
        {
            // Arrange
            var tower1 = new HeavyLaserTower(1, 1);
            var tower2 = new HeavyIceTower(1, 1);

            tower1.AppliedUpgrades.Add(TowerUpgrades.Burst);
            tower2.AppliedUpgrades.Add(TowerUpgrades.DoubleBullet);

            _gameState.Map.Towers.Add(tower1);
            _gameState.Map.Towers.Add(tower2);

            var expectedTowersCount = _gameState.Map.Towers.Count;

            // Act
            var result = _gameState.GetMapTowers() as IList;

            // Assert
            Assert.AreEqual(expectedTowersCount, result.Count, "GetMapTowers should return all towers");

            var towerResult1 = result[0];
            var towerResult2 = result[1];

            var appliedUpgrades1 = towerResult1.GetType().GetProperty("AppliedUpgrades").GetValue(towerResult1) as IList;
            var appliedUpgrades2 = towerResult2.GetType().GetProperty("AppliedUpgrades").GetValue(towerResult2) as IList;

            CollectionAssert.AreEqual(
                new List<string> { "Burst" },
                appliedUpgrades1.Cast<string>().ToList(),
                "First tower's upgrades should include 'Burst'"
            );

            CollectionAssert.AreEqual(
                new List<string> { "DoubleBullet" },
                appliedUpgrades2.Cast<string>().ToList(),
                "Second tower's upgrades should include 'DoubleBullet'"
            );
        }

    }
}
