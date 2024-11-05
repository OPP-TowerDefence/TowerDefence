using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class GameStateConstructor : GameStateTests
    {
        [TestMethod]
        public void GameState_ReturnsMapWithDefaultValues_WhenInitialized()
        {
            // Assert
            Assert.IsNotNull(_gameState.Map, "Map should not be null");
            Assert.AreEqual(10, _gameState.Map.Width, "Map width should be 10");
            Assert.AreEqual(10, _gameState.Map.Height, "Map height should be 10");
            Assert.IsNotNull(_gameState.Map.MainObject, "Map's MainObject should not be null");
            Assert.IsNotNull(_gameState.Map.Enemies, "Map's Enemies list should not be null");
            Assert.IsNotNull(_gameState.Map.Towers, "Map's Towers list should not be null");
            Assert.IsNotNull(_gameState.Map.Bullets, "Map's Bullets list should not be null");
        }

        [TestMethod]
        public void GameState_GameStartedIsFalse_WhenInitialized()
        {
            // Assert
            Assert.IsFalse(_gameState.GameStarted, "GameStarted should be false after initialization");
        }

        [TestMethod]
        public void GameState_PlayersListIsEmpty_WhenInitialized()
        {
            // Assert
            Assert.IsNotNull(_gameState.Players, "Players list should not be null");
            Assert.AreEqual(0, _gameState.Players.Count, "Players list should be empty after initialization");
        }

        [TestMethod]
        public void GameState_AvailableTowerTypesContainsAllTypes_WhenInitialized()
        {
            // Arrange

            // Act
            var field = typeof(GameState).GetField("_availableTowerTypes", BindingFlags.NonPublic | BindingFlags.Instance);
            var availableTowerTypes = field.GetValue(_gameState) as List<TowerTypes>;

            // Assert
            Assert.IsNotNull(availableTowerTypes, "_availableTowerTypes should not be null");
            var expectedTowerTypes = Enum.GetValues(typeof(TowerTypes)).Cast<TowerTypes>().ToList();
            CollectionAssert.AreEquivalent(expectedTowerTypes, availableTowerTypes, "_availableTowerTypes should contain all tower types");
        }

        [TestMethod]
        public void GameState_HubContextIsSet_WhenConstructed()
        {
            // Arrange

            // Act
            var field = typeof(GameState).GetField("_hubContext", BindingFlags.NonPublic | BindingFlags.Instance);
            var hubContext = field.GetValue(_gameState);

            // Assert
            Assert.IsNotNull(hubContext, "_hubContext should not be null");
            Assert.AreSame(_mockHubContext.Object, hubContext, "_hubContext should be the same as the one passed in constructor");
        }

        [TestMethod]
        public void GameState_RoomCodeIsSet_WhenConstructed()
        {
            // Arrange

            // Act
            var field = typeof(GameState).GetField("_roomCode", BindingFlags.NonPublic | BindingFlags.Instance);
            var roomCode = field.GetValue(_gameState) as string;

            // Assert
            Assert.IsNotNull(roomCode, "_roomCode should not be null");
            Assert.AreEqual(_roomCode, roomCode, "_roomCode should be set correctly");
        }

        [TestMethod]
        public void GameState_LevelFacadeIsInitialized_WhenConstructed()
        {
            // Arrange

            // Act
            var field = typeof(GameState).GetField("_levelFacade", BindingFlags.NonPublic | BindingFlags.Instance);
            var levelFacade = field.GetValue(_gameState);

            // Assert
            Assert.IsNotNull(levelFacade, "_levelFacade should not be null");
        }

        [TestMethod]
        public void GameState_ResourceManagerIsInitialized_WhenConstructed()
        {
            // Arrange

            // Act
            var field = typeof(GameState).GetField("_resourceManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var resourceManager = field.GetValue(_gameState);

            // Assert
            Assert.IsNotNull(resourceManager, "_resourceManager should not be null");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameState_ThrowsArgumentNullException_WhenHubContextIsNull()
        {
            // Arrange
            IHubContext<GameHub> nullHubContext = null;

            // Act
            var gameState = new GameState(nullHubContext, _roomCode);

            // Assert - handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameState_ThrowsArgumentNullException_WhenRoomCodeIsNull()
        {
            // Arrange
            string nullRoomCode = null;

            // Act
            var gameState = new GameState(_mockHubContext.Object, nullRoomCode);

            // Assert - handled by ExpectedException
        }
    }
}
