using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.SignalR;
using TowerDefense.Models;

namespace TowerDefense.Tests
{
    [TestClass]
    public class GameStateTests
    {
        protected GameState _gameState;
        private Mock<IHubContext<GameHub>> _mockHubContext;
        private string _roomCode = "TestRoom";

        [TestInitialize]
        public void Setup()
        {
            _mockHubContext = new Mock<IHubContext<GameHub>>();
            _gameState = new GameState(_mockHubContext.Object, _roomCode);
        }

        [TestCleanup]
        public void Teardown()
        {
            _gameState = null;
            _mockHubContext = null;
        }
    }
}
