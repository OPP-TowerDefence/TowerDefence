using Microsoft.AspNetCore.SignalR;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models
{
    public class Player : IResourceObserver
    {
        public string ConnectionId { get; set; }
        public int RemainingTowerRemovals { get; set; } = 0;
        public TowerTypes TowerType { get; set; }
        public string Username { get; set; }

        private readonly IClientProxy _clientProxy;
        private readonly ITowerFactory _towerFactory;

        public Player(string username, string connectionId, TowerTypes towerType, IHubContext<GameHub> hubContext)
        {
            Username = username;
            ConnectionId = connectionId;
            TowerType = towerType;

            _clientProxy = hubContext.Clients.Client(connectionId);

            _towerFactory = towerType switch
            {
                TowerTypes.Flame => new FlameTowerFactory(),
                TowerTypes.Ice => new IceTowerFactory(),
                TowerTypes.Laser => new LaserTowerFactory(),
                _ => new FlameTowerFactory(),
            };

            Logger.Instance.LogInfo($"{username} has received {towerType} towers");
        }

        public Tower CreateTower(int x, int y, TowerCategories towerCategory)
        {
            return towerCategory switch
            {
                TowerCategories.LongDistance => _towerFactory.CreateLongDistanceTower(x, y),
                TowerCategories.Heavy => _towerFactory.CreateHeavyTower(x, y),
                _ => _towerFactory.CreateLongDistanceTower(x, y),
            };
        }

        public async void OnResourceChanged(int resources)
        {
            await _clientProxy.SendAsync("OnResourceChanged", resources);
        }
    }
}
