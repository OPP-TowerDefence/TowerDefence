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

        public abstract TowerBuilder BuildBase(int x, int y);
        public abstract TowerBuilder AddWeapon();
        public abstract TowerBuilder AddArmor();
        public abstract Tower GetResult();
    }
}
