using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class RemovePlayer : GameStateTests
    {
        [TestMethod]
        public void RemovePlayer_ReturnsNothing_WhenPlayerDoesNotExist()
        {
            // Arrange
            var connectionId = "TestConnectionId";
            var playerCountBefore = _gameState.Players.Count;

            // Act
            _gameState.RemovePlayer(connectionId);

            // Assert
            Assert.AreEqual(playerCountBefore, _gameState.Players.Count, "Player count should not change as specified player was not in game.");
        }

        [TestMethod]
        public void RemovePlayer_RemovesPlayer_WhenPlayerIsInGame()
        {
            // Arrange
            var username = "TestUsername";
            var connectionId = "TestConnectionId";

            _gameState.AddPlayer(username, connectionId);

            // Act
            _gameState.RemovePlayer(connectionId);

            // Assert
            Assert.IsTrue(_gameState.Players
                .Where(p => string.Equals(p.Username, username) && string.Equals(p.ConnectionId, connectionId))
                .Count() == 0,
                "Player should be removed as specified player is in game."
            );
        }
    }
}
