using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Models;
using TowerDefense.Tests;

namespace Tests
{
    [TestClass]
    public class NotifyLevelChangeTests : GameStateTests
    {
        [TestMethod]
        public void NotifyLevelChange_SendLevelChangedMessage_WhenLevelIsIncreased()
        {
            // Arrange
            int newLevel = 2;
            var method = typeof(GameState).GetMethod("NotifyLevelChange", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act        
            method.Invoke(_gameState, new object[] { newLevel });

            // Assert
            _mockClientProxy.Verify(
                client => client.SendCoreAsync("LevelChanged", new object[] { newLevel }, It.IsAny<CancellationToken>()),
                Times.Once,
                "SendCoreAsync should be called with 'LevelChanged' and the correct level."
            );
        }
    }
}
