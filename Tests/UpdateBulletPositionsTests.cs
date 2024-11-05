using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class UpdateBulletPositionsTests : GameStateTests
    {
        [TestMethod]
        public void UpdateBulletPositions_MoveBulletsAndDamageEnemies_WhenEnemyIsInRange()
        {
            // Arrange
            var enemy = new FastEnemy(0, 0)
            {
                Id = Guid.NewGuid(),
                Health = 20
            };

            var bullet = new Bullet(0, 0, enemy.Id, damage: 10, speed: 2);

            _gameState.Map.Enemies.Add(enemy);
            _gameState.Map.Bullets.Add(bullet);

            var method = typeof(GameState).GetMethod("UpdateBulletPositions", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(_gameState, null);

            // Assert
            Assert.IsFalse(_gameState.Map.Bullets.Contains(bullet), "Bullet should be removed after hitting the enemy.");
            Assert.AreEqual(10, enemy.Health, "Enemy health should be reduced by the bullet's damage amount.");
        }

        [TestMethod]
        public void UpdateBulletPositions_NotHitEnemy_WhenEnemyIsOutOfRange()
        {
            // Arrange
            var enemy = new FastEnemy(100, 100)
            {
                Id = Guid.NewGuid(),
                Health = 20
            };

            var bullet = new Bullet(0, 0, enemy.Id, damage: 10, speed: 2);

            _gameState.Map.Enemies.Add(enemy);
            _gameState.Map.Bullets.Add(bullet);

            var method = typeof(GameState).GetMethod("UpdateBulletPositions", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(_gameState, null);

            // Assert
            Assert.IsTrue(_gameState.Map.Bullets.Contains(bullet), "Bullet should remain on the map as it did not reach the enemy.");
            Assert.AreEqual(20, enemy.Health, "Enemy health should remain unchanged as the bullet did not reach it.");
        }

        [TestMethod]
        public void UpdateBulletPositions_RemoveBulletAndEnemy_WhenBulletKillsEnemy()
        {
            // Arrange
            var enemy = new FastEnemy(0, 0)
            {
                Id = Guid.NewGuid(),
                Health = 10
            };

            var bullet = new Bullet(0, 0, enemy.Id, damage: 10, speed: 5);

            _gameState.Map.Enemies.Add(enemy);
            _gameState.Map.Bullets.Add(bullet);

            var method = typeof(GameState).GetMethod("UpdateBulletPositions", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(_gameState, null);

            // Assert
            Assert.IsFalse(_gameState.Map.Bullets.Contains(bullet), "Bullet should be removed after killing the enemy.");
            Assert.IsFalse(_gameState.Map.Enemies.Contains(enemy), "Enemy should be removed from the map after being killed.");
        }

        [TestMethod]
        public void UpdateBulletPositions_ReturnsNothing_WhenNoBulletsPresent()
        {
            // Arrange
            _gameState.Map.Bullets.Clear();
            _gameState.Map.Enemies.Clear();

            var method = typeof(GameState).GetMethod("UpdateBulletPositions", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(_gameState, null);

            // Assert
            Assert.AreEqual(0, _gameState.Map.Bullets.Count, "No bullets should be present.");
            Assert.AreEqual(0, _gameState.Map.Enemies.Count, "No enemies should be present.");
        }

        [TestMethod]
        public void UpdateBulletPositions_RemovesBullet_WhenNoMatchingEnemy()
        {
            // Arrange
            var enemy = new FastEnemy(0, 0)
            {
                Id = Guid.NewGuid(),
                Health = 20
            };

            var bullet = new Bullet(0, 0, Guid.NewGuid(), damage: 10, speed: 2);

            _gameState.Map.Enemies.Add(enemy);
            _gameState.Map.Bullets.Add(bullet);

            var method = typeof(GameState).GetMethod("UpdateBulletPositions", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(_gameState, null);

            // Assert
            Assert.IsFalse(_gameState.Map.Bullets.Contains(bullet), "Bullet should be removed if no matching enemy is found.");
            Assert.IsTrue(_gameState.Map.Enemies.Contains(enemy), "Enemy should remain as it wasn't targeted by any bullet.");
        }
    }
}
