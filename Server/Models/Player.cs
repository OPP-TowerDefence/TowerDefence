using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models
{
    public class Player
    {
        public TowerTypes TowerType { get; set; }
        public string Username { get; set; }
        public string ConnectionId { get; set; }
        private readonly ITowerFactory _towerFactory;

        public Player(string username, string connectionId, TowerTypes towerType)
        {
            TowerType = towerType;
            Username = username;
            ConnectionId = connectionId;

            switch (towerType)
            {
                case TowerTypes.Flame:
                    _towerFactory = new FlameTowerFactory();
                    break;
                case TowerTypes.Ice:
                    _towerFactory = new IceTowerFactory();
                    break;
                case TowerTypes.Laser:
                    _towerFactory = new LaserTowerFactory();
                    break;
                default:
                    _towerFactory = new FlameTowerFactory();
                    break;

            }

            Logger.Instance.LogInfo($"{username} has received {towerType.ToString()} towers");
        }

        public Tower CreateTower(int x, int y, TowerCategories towerCategory)
        {
            switch(towerCategory)
            {
                case TowerCategories.LongDistance:
                    return _towerFactory.CreateLongDistanceTower(x, y);
                case TowerCategories.Heavy:
                    return _towerFactory.CreateHeavyTower(x, y);
                default:
                    return _towerFactory.CreateLongDistanceTower(x, y);
            }
        }
    }
}
