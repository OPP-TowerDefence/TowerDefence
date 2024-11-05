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
        protected Mock<IHubContext<GameHub>> _mockHubContext;
        protected Mock<IClientProxy> _mockClientProxy;
        protected string _roomCode = "TestRoom";

        [TestInitialize]
        public void Setup()
        {
            _mockHubContext = new Mock<IHubContext<GameHub>>();
            _mockClientProxy = new Mock<IClientProxy>();

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
            _mockHubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);

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