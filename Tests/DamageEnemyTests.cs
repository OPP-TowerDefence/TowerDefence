using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Models.Enemies;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class DamageEnemyTests : GameStateTests
    {
        [TestMethod]
        public void DamageEnemy_ReduceHealth_WhenEnemyHasTakenDamage()
        {
            // Arrange
            var enemy = new FastEnemy(0, 0)
            {
                Health = 10
            };

            var method = typeof(GameState).GetMethod("DamageEnemy", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act          
            method.Invoke(_gameState, new object[] { enemy, 5 });

            // Assert
            Assert.AreEqual(5, enemy.Health, "Enemy health should be reduced by the damage amount.");
        }

        [TestMethod]
        public void DamageEnemy_RemoveEnemy_WhenHealthIsZeroOrLess()
        {
            // Arrange
            var enemy = new FastEnemy(0, 0)
            {
                Health = 10
            };
            _gameState.Map.Enemies.Add(enemy);

            var method = typeof(GameState).GetMethod("DamageEnemy", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(_gameState, new object[] { enemy, 10 });

            // Assert
            Assert.IsFalse(_gameState.Map.Enemies.Contains(enemy), "Enemy should be removed from the map when health reaches zero.");
        }
    }
}
