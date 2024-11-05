﻿using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class StartGameTests : GameStateTests
    {
        [TestMethod]
        public void StartGame_ThrowException_WhenUserIsNotInGame()
        {
            // Arrange
            var username = "TestUsername";

            // Act & Assert
            Assert.ThrowsException<Exception>(() => _gameState.StartGame(username), "Exception should be thrown as user is not in game.");
        }

        [TestMethod]
        public void StartGame_StartsGame_WhenUsersIsInGame()
        {
            // Arrange
            var username = "TestUsername";
            var connectionId = "TestConnectionId";

            _gameState.AddPlayer(username, connectionId);

            // Act
            _gameState.StartGame(connectionId);

            // Assert
            Assert.IsTrue(_gameState.GameStarted, "Game should be started as user is in game.");
        }
    }
}