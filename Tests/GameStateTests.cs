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

        private Mock<ISingleClientProxy> _mockClientProxy;
        private string _roomCode = "TestRoom";

        [TestInitialize]
        public void Setup()
        {
            _mockHubContext = new Mock<IHubContext<GameHub>>();
            _mockClientProxy = new Mock<ISingleClientProxy>();

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.Client(It.IsAny<string>())).Returns(_mockClientProxy.Object);
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
