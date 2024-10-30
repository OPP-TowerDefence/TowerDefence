using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class PlaceTower : GameStateTests
    {
        [TestMethod]
        public void PlaceTower_ThrowArgumentNullException_WhenPlayerIsNull()
        {
            // Arrange
            var x = 1;
            var y = 1;
            var towerCategory = TowerCategories.Heavy;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _gameState.PlaceTower(x, y, towerCategory, null), "ArgumentNullException should be thrown as player placing the tower is null");
        }

        [DataTestMethod]
        [DataRow(TowerCategories.Heavy)]
        [DataRow(TowerCategories.LongDistance)]
        public void PlaceTower_PlacesTower_WhenPlayerIsNotNull(TowerCategories towerCategory)
        {
            // Arrange
            var x = 1;
            var y = 1;
            var player = new Player("TestUsername", "TestConnectionId", TowerTypes.Flame, _mockHubContext.Object);
            var towerCountBefore = _gameState.Map.Towers.Count;

            // Act
            _gameState.PlaceTower(x, y, towerCategory, player);

            // Assert
            Assert.IsTrue(_gameState.Map.Towers.Count > towerCountBefore, "Tower count should increase when tower is placed.");
        }
    }
}
