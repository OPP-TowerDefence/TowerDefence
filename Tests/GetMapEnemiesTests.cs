using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Models;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class GetMapEnemiesTests : GameStateTests
    {
        [TestMethod]
        public void GetMapEnemies_ReturnsEmptyList_WhenNoEnemies()
        {
            // Arrange
            _gameState.Map.Enemies.Clear();
            var expectedEnemiesCount = 0;

            // Act
            var result = _gameState.GetMapEnemies() as IList;

            // Assert
            Assert.AreEqual(expectedEnemiesCount, result.Count, "GetMapEnemies should return an empty list when no enemies are present");
        }

        [TestMethod]
        public void GetMapEnemies_ReturnsMapEnemies_ThereAreEnemies()
        {
            // Arrange
            _gameState.Map.Enemies.Add(new FlyingEnemy(1,1));
            _gameState.Map.Enemies.Add(new FlyingEnemy(1,1));
            _gameState.Map.Enemies.Add(new FlyingEnemy(1,1));
            var expectedEnemiesCount = _gameState.Map.Enemies.Count;

            // Act
            var result = _gameState.GetMapEnemies() as IList;

            // Assert
            Assert.AreEqual(expectedEnemiesCount, result.Count, "GetMapEnemies should return all enemies");
        }
    }
}
