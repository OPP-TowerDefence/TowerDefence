using System.Runtime.CompilerServices;
using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public abstract class HeavyTower : Tower
    {
        public override TowerCategories Category => TowerCategories.Heavy;
        public HeavyTower(int x, int y) : base(x, y)
        {
        }
    }
}
