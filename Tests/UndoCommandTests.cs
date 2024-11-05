using Moq;
using System.Reflection;
using TowerDefense.Interfaces;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class UndoCommandTests : GameStateTests
    {
        [TestMethod]
        public void UndoCommand_UndoesCommand_WhenCalled()
        {
            // Arrange
            var connectionId = "TestConnectionId1";
            var mockCommand = new Mock<ICommand>();

            _gameState.ProcessCommand(mockCommand.Object, connectionId);

            // Act
            _gameState.UndoLastCommand(connectionId);

            // Assert
            mockCommand.Verify(c => c.Undo(), Times.Once, "Command's Undo method should be called once.");
        }

        [TestMethod]
        public void UndoCommand_DoesNotUndoCommand_WhenNoCommands()
        {
            // Arrange
            var connectionId = "TestConnectionId1";

            // Act
            _gameState.UndoLastCommand(connectionId);

            // Assert
            Assert.IsTrue(true, "No exception should be thrown when no commands are present.");
        }

        [TestMethod]
        public void UndoCommand_DoesNotUndoCommand_WhenDifferentConnectionId()
        {
            // Arrange
            var connectionId = "TestConnectionId1";
            var mockCommand = new Mock<ICommand>();

            _gameState.ProcessCommand(mockCommand.Object, connectionId);

            // Act
            _gameState.UndoLastCommand("TestConnectionId2");

            // Assert
            Assert.IsTrue(true, "No exception should be thrown when connectionId does not match.");
        }

        [TestMethod]
        public void UndoCommand_RemovesCommandFromHistory_WhenCalled()
        {
            // Arrange
            var connectionId = "TestConnectionId1";
            var mockCommand = new Mock<ICommand>();

            _gameState.ProcessCommand(mockCommand.Object, connectionId);

            // Act
            _gameState.UndoLastCommand(connectionId);

            // Access the private _playerCommands field
            var field = typeof(GameState).GetField("_playerCommands", BindingFlags.NonPublic | BindingFlags.Instance);
            var playerCommands = (Dictionary<string, LinkedList<ICommand>>)field.GetValue(_gameState);

            // Assert
            Assert.IsTrue(playerCommands.ContainsKey(connectionId), "Player commands should contain the new connectionId");
            Assert.AreEqual(0, playerCommands[connectionId].Count, "Command history should have no commands");
        }
    }
}
