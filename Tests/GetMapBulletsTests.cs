using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class GetMapBulletsTests : GameStateTests
    {
        [TestMethod]
        public void GetMapBullets_ReturnsEmptyList_WhenNoBullets()
        {
            // Arrange
            _gameState.Map.Bullets.Clear();
            var expectedBulletsCount = 0;

            // Act
            var result = _gameState.GetMapBullets() as IList;

            // Assert
            Assert.AreEqual(expectedBulletsCount, result.Count, "GetMapBullets should return an empty list when no bullets are present");      
        }

        [TestMethod]
        public void GetMapBullets_ReturnsMapBullets_ThereAreBullets()
        {
            // Arrange
            _gameState.Map.Bullets.Add(new Bullet(1, 1, Guid.Empty, 1, 1));
            _gameState.Map.Bullets.Add(new Bullet(1, 1, Guid.Empty, 1, 1));
            _gameState.Map.Bullets.Add(new Bullet(1, 1, Guid.Empty, 1, 1));
            var expectedBulletsCount = _gameState.Map.Bullets.Count;

            // Act
            var result = _gameState.GetMapBullets() as IList;

            // Assert
            Assert.AreEqual(expectedBulletsCount, result.Count, "GetMapBullets should return all bullets");
        }
    }
}
