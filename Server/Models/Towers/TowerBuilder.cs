using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public abstract class TowerBuilder
    {
        protected Tower _tower;
        protected TowerCategories _category;

        public TowerBuilder(TowerCategories category)
        {
            _category = category;
        }

        public abstract void BuildBase(int x, int y);
        public abstract void AddWeapon();
        public abstract void AddArmor();
        public abstract Tower GetResult();
    }
}
