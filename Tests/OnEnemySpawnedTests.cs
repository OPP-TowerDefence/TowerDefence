using System.Reflection;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class OnEnemySpawnedTests : GameStateTests
    {
        [TestMethod]
        public void OnEnemySpawned_IncreaseEnemyCount_EnemiesAreSpawned()
        {
            // Arrange
            var enemiesSpawnedField = typeof(GameState).GetField("_enemiesSpawned", BindingFlags.NonPublic | BindingFlags.Instance);
            var enemyCountBefore = (int)enemiesSpawnedField.GetValue(_gameState);
            var method = typeof(GameState).GetMethod("OnEnemySpawned", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(_gameState, null);

            // Assert
            Assert.IsTrue((int)enemiesSpawnedField.GetValue(_gameState) > enemyCountBefore, "Enemy count should increase when enemies are spawned.");
        }

        [TestMethod]
        public void OnEnemySpawned_IncreaseLevelCount_LevelUpEnemyCountEnemiesSpawned()
        {
            // Arrange
            var baseEnemiesField = typeof(GameState).GetField("_baseEnemiesPerLevel", BindingFlags.NonPublic | BindingFlags.Static);
            var currentLevelField = typeof(GameState).GetField("_currentLevel", BindingFlags.NonPublic | BindingFlags.Instance);

            var currentLevelBefore = (int)currentLevelField.GetValue(_gameState);

            var method = typeof(GameState).GetMethod("OnEnemySpawned", BindingFlags.NonPublic | BindingFlags.Instance);

            var levelUpEnemyCount = (int)baseEnemiesField.GetValue(_gameState) * currentLevelBefore * (currentLevelBefore + 1) / 2;

            // Act
            for (int i = 0; i < levelUpEnemyCount; i++)
            {
                method.Invoke(_gameState, null);
            }

            // Assert
            Assert.IsTrue((int)currentLevelField.GetValue(_gameState) > currentLevelBefore, $"Level should increase when the calculated amount {levelUpEnemyCount} of enemies are spawned.");
        }

        [TestMethod]
        public void OnEnemySpawned_PersistLevelCount_LessThanLevelUpEnemyCountEnemiesSpawned()
        {
            // Arrange
            var baseEnemiesField = typeof(GameState).GetField("_baseEnemiesPerLevel", BindingFlags.NonPublic | BindingFlags.Static);
            var currentLevelField = typeof(GameState).GetField("_currentLevel", BindingFlags.NonPublic | BindingFlags.Instance);

            var currentLevelBefore = (int)currentLevelField.GetValue(_gameState);

            var method = typeof(GameState).GetMethod("OnEnemySpawned", BindingFlags.NonPublic | BindingFlags.Instance);

            var levelUpEnemyCount = (int)baseEnemiesField.GetValue(_gameState) * currentLevelBefore * (currentLevelBefore + 1) / 2;
            
            // Act
            for (int i = 0; i < levelUpEnemyCount - 1; i++)
            {
                method.Invoke(_gameState, null);
            }

            // Assert
            Assert.IsTrue((int)currentLevelField.GetValue(_gameState) == currentLevelBefore, $"Level should remain the same when lass than the calculated amount {levelUpEnemyCount} of enemies are spawned.");
        }

        [TestMethod]
        public void OnEnemySpawned_OnLevelChangedInvoked_LevelUpEnemyCountEnemiesSpawned()
        {
            // Arrange
            var enemiesSpawnedField = typeof(GameState).GetField("_enemiesSpawned", BindingFlags.NonPublic | BindingFlags.Instance);
            var baseEnemiesField = typeof(GameState).GetField("_baseEnemiesPerLevel", BindingFlags.NonPublic | BindingFlags.Static);
            var currentLevelField = typeof(GameState).GetField("_currentLevel", BindingFlags.NonPublic | BindingFlags.Instance);

            var currentLevel = (int)currentLevelField.GetValue(_gameState);

            int levelUpEnemyCount = (int)baseEnemiesField.GetValue(null) * currentLevel * (currentLevel + 1) / 2;

            var method = typeof(GameState).GetMethod("OnEnemySpawned", BindingFlags.NonPublic | BindingFlags.Instance);

            bool eventInvoked = false;
            _gameState.OnLevelChanged += (newLevel) => { eventInvoked = true; };

            // Act
            for (int i = 0; i < levelUpEnemyCount; i++)
            {
                method.Invoke(_gameState, null);
            }

            // Assert
            Assert.IsTrue(eventInvoked, "OnLevelChanged event should be invoked when level changes.");
        }

        [TestMethod]
        public void OnEnemySpawned_OnLevelChangedNotInvoked_LessThanLevelUpEnemyCountEnemiesSpawned()
        {
            // Arrange
            var enemiesSpawnedField = typeof(GameState).GetField("_enemiesSpawned", BindingFlags.NonPublic | BindingFlags.Instance);
            var baseEnemiesField = typeof(GameState).GetField("_baseEnemiesPerLevel", BindingFlags.NonPublic | BindingFlags.Static);
            var currentLevelField = typeof(GameState).GetField("_currentLevel", BindingFlags.NonPublic | BindingFlags.Instance);

            var currentLevel = (int)currentLevelField.GetValue(_gameState);

            int levelUpEnemyCount = (int)baseEnemiesField.GetValue(null) * currentLevel * (currentLevel + 1) / 2;

            var method = typeof(GameState).GetMethod("OnEnemySpawned", BindingFlags.NonPublic | BindingFlags.Instance);

            bool eventInvoked = false;
            _gameState.OnLevelChanged += (newLevel) => { eventInvoked = true; };

            // Act
            for (int i = 0; i < levelUpEnemyCount - 1; i++)
            {
                method.Invoke(_gameState, null);
            }

            // Assert: Ensure the level increased and the event was invoked
            Assert.IsFalse(eventInvoked, "OnLevelChanged event should be invoked when level changes.");
        }


        [TestMethod]
        public void OnEnemySpawned_ThrowArgumentNullException_OnLevelChangedIsNull()
        {
            // Arrange
            var enemiesSpawnedField = typeof(GameState).GetField("_enemiesSpawned", BindingFlags.NonPublic | BindingFlags.Instance);
            var baseEnemiesField = typeof(GameState).GetField("_baseEnemiesPerLevel", BindingFlags.NonPublic | BindingFlags.Static);
            var currentLevelField = typeof(GameState).GetField("_currentLevel", BindingFlags.NonPublic | BindingFlags.Instance);

            var currentLevel = (int)currentLevelField.GetValue(_gameState);

            int levelUpEnemyCount = (int)baseEnemiesField.GetValue(null) * currentLevel * (currentLevel + 1) / 2;

            var method = typeof(GameState).GetMethod("OnEnemySpawned", BindingFlags.NonPublic | BindingFlags.Instance);

            typeof(GameState)
                .GetField("OnLevelChanged", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_gameState, null);

            // Act & Assert
            var exception = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                for (int i = 0; i < levelUpEnemyCount; i++)
                {
                    method.Invoke(_gameState, null);
                }
            });

            Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
        }
    }
}
