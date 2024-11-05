using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;
using TowerDefense.Models;
using TowerDefense.Models.Commands;
using TowerDefense.Models.Towers;
using TowerDefense.Tests;
using TowerDefense.Utils;

namespace Tests
{
    [TestClass]
    public class ProcessCommandTest : GameStateTests
    {
        [TestMethod]
        public void ProcessCommand_AddsCommandToHistory_WhenNewConnectionId()
        {
            // Arrange
            var connectionId = "Conn1";
            var mockCommand = new Mock<ICommand>();

            // Act
            _gameState.ProcessCommand(mockCommand.Object, connectionId);

            // Access the private _playerCommands field
            var field = typeof(GameState).GetField("_playerCommands", BindingFlags.NonPublic | BindingFlags.Instance);
            var playerCommands = (Dictionary<string, LinkedList<ICommand>>)field.GetValue(_gameState);

            // Assert
            Assert.IsTrue(playerCommands.ContainsKey(connectionId), "Player commands should contain the new connectionId");
            Assert.AreEqual(1, playerCommands[connectionId].Count, "Command history should have one command");
            Assert.AreSame(mockCommand.Object, playerCommands[connectionId].First.Value, "The command in history should be the one processed");
        }

        [TestMethod]
        public void ProcessCommand_ExecutesCommand_WhenCalled()
        {
            // Arrange
            var connectionId = "Conn1";
            var mockCommand = new Mock<ICommand>();

            // Act
            _gameState.ProcessCommand(mockCommand.Object, connectionId);

            // Assert
            mockCommand.Verify(c => c.Execute(), Times.Once, "Command's Execute method should be called once");
        }

        [TestMethod]
        public void ProcessCommand_RemovesOldestCommand_WhenHistoryLimitExceeded()
        {
            // Arrange
            var connectionId = "Conn1";
            var mockCommands = new List<Mock<ICommand>>();
            int commandHistoryLimit = 3;

            // Access the private _commandHistoryLimit field
            var historyLimitField = typeof(GameState).GetField("_commandHistoryLimit", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            commandHistoryLimit = (int)historyLimitField.GetValue(_gameState);

            // Create more commands than the history limit
            for (int i = 0; i < commandHistoryLimit + 1; i++)
            {
                mockCommands.Add(new Mock<ICommand>());
            }

            // Act
            foreach (var mockCommand in mockCommands)
            {
                _gameState.ProcessCommand(mockCommand.Object, connectionId);
            }

            // Access the private _playerCommands field
            var field = typeof(GameState).GetField("_playerCommands", BindingFlags.NonPublic | BindingFlags.Instance);
            var playerCommands = (Dictionary<string, LinkedList<ICommand>>)field.GetValue(_gameState);
            var commandsHistory = playerCommands[connectionId];

            // Assert
            Assert.AreEqual(commandHistoryLimit, commandsHistory.Count, $"Command history should not exceed the limit of {commandHistoryLimit}");
            // The oldest command should be removed (commands are added to the front)
            Assert.IsFalse(commandsHistory.Contains(mockCommands[0].Object), "Oldest command should be removed from the history");
        }

        [TestMethod]
        public void ProcessCommand_MaintainsSeparateHistories_ForDifferentPlayers()
        {
            // Arrange
            var connectionId1 = "Conn1";
            var connectionId2 = "Conn2";
            var mockCommand1 = new Mock<ICommand>();
            var mockCommand2 = new Mock<ICommand>();

            // Act
            _gameState.ProcessCommand(mockCommand1.Object, connectionId1);
            _gameState.ProcessCommand(mockCommand2.Object, connectionId2);

            // Access the private _playerCommands field
            var field = typeof(GameState).GetField("_playerCommands", BindingFlags.NonPublic | BindingFlags.Instance);
            var playerCommands = (Dictionary<string, LinkedList<ICommand>>)field.GetValue(_gameState);

            // Assert
            Assert.IsTrue(playerCommands.ContainsKey(connectionId1), "Player commands should contain connectionId1");
            Assert.IsTrue(playerCommands.ContainsKey(connectionId2), "Player commands should contain connectionId2");
            Assert.AreEqual(1, playerCommands[connectionId1].Count, "ConnectionId1 should have one command");
            Assert.AreEqual(1, playerCommands[connectionId2].Count, "ConnectionId2 should have one command");
            Assert.AreSame(mockCommand1.Object, playerCommands[connectionId1].First.Value, "Command1 should be in connectionId1's history");
            Assert.AreSame(mockCommand2.Object, playerCommands[connectionId2].First.Value, "Command2 should be in connectionId2's history");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessCommand_ThrowsArgumentException_WhenConnectionIdIsNullOrEmpty(string connectionId)
        {
            // Arrange
            var mockCommand = new Mock<ICommand>();

            // Act
            _gameState.ProcessCommand(mockCommand.Object, connectionId);

            // Assert - Handled by ExpectedException
        }

        [TestMethod]
        public void ProcessCommand_CanProcessPlaceTowerCommand_Correctly()
        {
            // Arrange
            var connectionId = "Conn1";
            var towerX = 1;
            var towerY = 1;
            var tower = new HeavyIceTower(towerX, towerY);

            // Use an actual Map instance
            var map = new Map(10, 10);

            // Mock LevelProgressionFacade if necessary
            var mockLevelFacade = new Mock<LevelProgressionFacade>(map.MainObject, map.Enemies, map.Towers);

            // Create PlaceTowerCommand
            var placeTowerCommand = new PlaceTowerCommand(map, tower, mockLevelFacade.Object);

            // Act
            _gameState.ProcessCommand(placeTowerCommand, connectionId);

            // Assert
            Assert.IsTrue(map.Towers.Contains(tower), "Tower should be added to the map");          
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessCommand_DoesNotAddCommand_WhenCommandIsNull()
        {
            // Arrange
            var connectionId = "Conn1";
            ICommand nullCommand = null;

            // Act and Assert
            _gameState.ProcessCommand(nullCommand, connectionId);
        }

        [DataTestMethod]
        [DataRow("Conn1", 1)]
        [DataRow("Conn1", 2)]
        [DataRow("Conn1", 3)]
        [DataRow("Conn1", 4)] // This should exceed the history limit
        public void ProcessCommand_MaintainsCommandHistoryLimit_WhenAddingCommands(string connectionId, int numberOfCommands)
        {
            // Arrange
            var mockCommands = new List<Mock<ICommand>>();
            int commandHistoryLimit = 3;

            // Access the private _commandHistoryLimit field
            var historyLimitField = typeof(GameState).GetField("_commandHistoryLimit", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            commandHistoryLimit = (int)historyLimitField.GetValue(_gameState);

            // Create the specified number of commands
            for (int i = 0; i < numberOfCommands; i++)
            {
                mockCommands.Add(new Mock<ICommand>());
            }

            // Act
            foreach (var mockCommand in mockCommands)
            {
                _gameState.ProcessCommand(mockCommand.Object, connectionId);
            }

            // Access the private _playerCommands field
            var field = typeof(GameState).GetField("_playerCommands", BindingFlags.NonPublic | BindingFlags.Instance);
            var playerCommands = (Dictionary<string, LinkedList<ICommand>>)field.GetValue(_gameState);
            var commandsHistory = playerCommands[connectionId];

            // Assert
            int expectedCount = Math.Min(numberOfCommands, commandHistoryLimit);
            Assert.AreEqual(expectedCount, commandsHistory.Count, $"Command history should not exceed the limit of {commandHistoryLimit}");
        }
    }
}
