using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class AddPlayer : GameStateTests
    {

        [TestMethod]
        public void AddPlayer_ReturnNothing_WhenPlayerExists()
        {
            //Arrange
            _gameState.Players.Add(new Player("user1", "TestRoom", TowerTypes.Flame));
            var username = "user1";
            var connectionId = "TestRoom";

            //Act
            _gameState.AddPlayer(username, connectionId);

            //Assert
            Assert.AreEqual(1, _gameState.Players.Count, "Players list should not be changed");
        }

        [DataTestMethod]
        [DataRow("user1", "con1")]
        [DataRow("user2", "con2")]
        [DataRow("user3", "con3")]
        [DataRow("user4", "con4")]
        public void AddPlayer_ReturnNothing_WhenAvailableTowerTypesAreZero(string username, string connectionId)
        {
            // Arrange
            var field = typeof(GameState).GetField("_availableTowerTypes", BindingFlags.NonPublic | BindingFlags.Instance);
            var availableTowerTypes = field.GetValue(_gameState) as List<TowerTypes>;
            availableTowerTypes.Clear();

            int initialPlayerCount = _gameState.Players.Count;

            // Act
            _gameState.AddPlayer(username, connectionId);

            // Assert
            Assert.AreEqual(initialPlayerCount, _gameState.Players.Count, "Players list should remain the same when no tower types are available");
        }

        [DataTestMethod]
        [DataRow("Player1", "con1")]
        [DataRow("Player2", "con2")]
        [DataRow("Player3", "con3")]
        public void AddPlayer_AddsPlayer_WhenTowerTypesAvailable(string username, string connectionId)
        {
            // Arrange

            // Act
            _gameState.AddPlayer(username, connectionId);

            // Assert
            var player = _gameState.Players.FirstOrDefault(p => p.ConnectionId == connectionId);
            Assert.IsNotNull(player, $"Player with connectionId {connectionId} should be added");
            Assert.AreEqual(username, player.Username, "Player's username should match the one added");
            Assert.IsTrue(Enum.IsDefined(typeof(TowerTypes), player.TowerType), "Player's TowerType should be a valid TowerType");
        }

        [TestMethod]
        public void AddPlayer_AddPlayers_AllTowerTypesGetSelected()
        {
            // Arrange
            var field = typeof(GameState).GetField("_availableTowerTypes", BindingFlags.NonPublic | BindingFlags.Instance);
            var availableTowerTypes = field.GetValue(_gameState) as List<TowerTypes>;
            var availableTowerTypesBeforeSelection = new List<TowerTypes>(availableTowerTypes);
            var playersToAddCount = availableTowerTypesBeforeSelection.Count;

            // Act
            for (int i = 0; i < playersToAddCount; i++)
            {
                _gameState.AddPlayer($"Player{i}", $"con{i}");
            }

            // Assert
            var players = _gameState.Players;
            Assert.AreEqual(playersToAddCount, players.Count, "Player count should be equal to 3");

            var towerTypesSelected = players.Select(p => p.TowerType).Distinct().ToList();
            towerTypesSelected.ForEach(towerTypesSelected => Assert.IsTrue(availableTowerTypesBeforeSelection.Contains(towerTypesSelected), "TowerType should be taken from available TowerTypes"));
            towerTypesSelected.ForEach(towerTypesSelected => Assert.IsFalse(availableTowerTypes.Contains(towerTypesSelected), "TowerType should be removed from Available tower types after player addition"));
            Assert.AreEqual(0, availableTowerTypes.Count, "All TowerTypes should be selected");
        }

    }
}
