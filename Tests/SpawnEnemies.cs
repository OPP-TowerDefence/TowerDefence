using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class SpawnEnemies : GameStateTests
    {
        [TestMethod]
        public void SpawnEnemies_IncreaseEnemyCount_EnemiesAreSpawned()
        {
            // Arrange
            var enemyCountBefore = _gameState.Map.Enemies.Count;

            // Act
            _gameState.SpawnEnemies();
            
            // Assert
            Assert.IsTrue(_gameState.Map.Enemies.Count > enemyCountBefore, "Enemy count should increase change when enemies are spawned.");
        }
    }
}
