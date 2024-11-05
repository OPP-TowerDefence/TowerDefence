using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class TowerAttackTests : GameStateTests
    {
        [TestMethod]
        public void TowerAttack_ReturnsNothing_WhenNoEnemies()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            _gameState.Map.Towers.Add(tower);

            var bulletsBefore = _gameState.Map.Bullets.Count;

            // Act
            _gameState.TowerAttack();

            // Assert
            Assert.AreEqual(bulletsBefore, _gameState.Map.Bullets.Count, "No bullets should be added as there are no enemies.");
        }

        [TestMethod]
        public void TowerAttack_ReturnsNothing_WhenNoTowers()
        {
            // Arrange
            var enemy = new FastEnemy(5, 5);
            _gameState.Map.Enemies.Add(enemy);

            var bulletsBefore = _gameState.Map.Bullets.Count;

            // Act
            _gameState.TowerAttack();

            // Assert
            Assert.AreEqual(bulletsBefore, _gameState.Map.Bullets.Count, "No bullets should be added as there are no towers.");
        }

        [TestMethod]
        public void TowerAttack_AddsBullets_WhenTowersAndEnemiesExist()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            var enemy = new FastEnemy(5, 5);
            tower.TicksToShoot = 1;
            tower.Weapon = new Weapon("Flame cannon", 10, 10, 2);

            _gameState.Map.Towers.Add(tower);
            _gameState.Map.Enemies.Add(enemy);

            var bulletsBefore = _gameState.Map.Bullets.Count;

            // Act
            _gameState.TowerAttack();

            // Assert
            Assert.IsTrue(_gameState.Map.Bullets.Count > bulletsBefore, "Bullets should be added as towers shoot at enemies.");
        }

        [TestMethod]
        public void TowerAttack_ReturnsNothing_WhenEnemyOutOfRange()
        {
            // Arrange
            var tower = new HeavyFlameTower(0, 0);
            var enemy = new FastEnemy(100, 100);

            _gameState.Map.Towers.Add(tower);
            _gameState.Map.Enemies.Add(enemy);

            var bulletsBefore = _gameState.Map.Bullets.Count;

            // Act
            _gameState.TowerAttack();

            // Assert
            Assert.AreEqual(bulletsBefore, _gameState.Map.Bullets.Count, "No bullets should be added as enemy is out of range.");
        }
    }
}
