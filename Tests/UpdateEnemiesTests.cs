using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Models.Enemies;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class UpdateEnemiesTests : GameStateTests
    {
        [TestMethod]
        public void UpdateEnemies_MovesEnemiesTowardTarget_WhenTargetIsNotReached()
        {
            // Arrange
            var enemy = new FastEnemy(0, 0);

            _gameState.Map.Enemies.Add(enemy);

            // Act
            _gameState.UpdateEnemies();

            // Assert
            Assert.IsTrue(enemy.X != 0 || enemy.Y != 0, "Enemy should have moved from its initial position.");
            Assert.IsTrue(enemy.X > 0 && enemy.Y > 0, "Enemy should have moved closer to the target.");
        }


        [TestMethod]
        public void UpdateEnemies_DecreasesMainObjectHealth_WhenEnemyReachesTarget()
        {
            // Arrange
            var enemy = new FastEnemy(0, 0)
            {
                Speed = 10
            };

            int initialHealth = _gameState.Map.MainObject.Health;

            _gameState.Map.Enemies.Add(enemy);

            // Act
            _gameState.UpdateEnemies();

            // Assert
            Assert.AreEqual(initialHealth - 5, _gameState.Map.MainObject.Health, "Main object health should decrease by 5 when the enemy reaches the target.");
        }
    }
}
