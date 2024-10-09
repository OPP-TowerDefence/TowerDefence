using TowerDefense.Models.Towers;

namespace TowerDefense.Interfaces
{
    public interface ITowerFactory
    {
        public Tower CreateLongDistanceTower(int x, int y);
        public Tower CreateHeavyTower(int x, int y);
    }
}
