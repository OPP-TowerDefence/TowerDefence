using TowerDefense.Enums;

namespace TowerDefense.Models.Towers
{
    public abstract class TowerBuilder(TowerCategories category)
    {
        protected TowerCategories _category = category;
        protected Tower _tower;

        public abstract TowerBuilder AddArmor();

        public abstract TowerBuilder AddWeapon();

        public abstract TowerBuilder BuildBase(int x, int y);

        public abstract Tower GetResult();
    }
}
