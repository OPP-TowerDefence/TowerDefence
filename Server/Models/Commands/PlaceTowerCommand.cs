using TowerDefense.Interfaces;
using TowerDefense.Models.Towers;
using TowerDefense.Utils;

namespace TowerDefense.Models.Commands
{
    public class PlaceTowerCommand(Map map, Tower tower) : ICommand
    {
        private readonly Map _map = map;
        private readonly Tower _tower = tower;

        public void Execute()
        {
            if (!_map.IsOccupied(tower.X, tower.Y) && _map.IsValidPosition(tower.X, tower.Y))
            {
                _map.Towers.Add(_tower);
            }
            else
            {
                Logger.Instance.LogError($"Unable to place tower at position ({tower.X},{tower.Y}). Position is either occupied or invalid.");
            }
        }

        public void Undo()
        {
            _map.Towers.Remove(_tower);
        }
    }
}
