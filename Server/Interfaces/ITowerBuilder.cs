using Newtonsoft.Json.Bson;
using TowerDefense.Models;
using TowerDefense.Models.Towers;

namespace TowerDefense.Interfaces
{
    public interface ITowerBuilder
    {
        void BuildBase(int x, int y);
        void AddWeapon();
        void AddArmor();
        Tower GetResult();
    }
}
