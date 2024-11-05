using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class GetActivePlayersTests : GameStateTests
    {
        [TestMethod]
        public void GetActivePlayers_ReturnsEmptyList_WhenNoPlayers()
        {
            // Arrange
            _gameState.Players.Clear();
            var expectedPlayersCount = 0;

            // Act
            var result = _gameState.GetActivePlayers();

            // Assert
            Assert.AreEqual(expectedPlayersCount, result.Count(), "GetActivePlayers should return an empty list when no players are present");
        }

        [TestMethod]
        public void GetActivePlayers_ReturnsActivePlayers_ThereArePlayers()
        {
            // Arrange
            _gameState.Players.Add(new Player("user1", "con1", TowerTypes.Ice));
            _gameState.Players.Add(new Player("user2", "con2", TowerTypes.Flame));
            var expectedPlayersCount = _gameState.Players.Count;

            // Act
            var result = _gameState.GetActivePlayers();

            // Assert
            Assert.AreEqual(expectedPlayersCount, result.Count(), "GetActivePlayers should return all players");
        }
    }
}
