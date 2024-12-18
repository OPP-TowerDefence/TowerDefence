﻿using TowerDefense.Models.Enemies;
using TowerDefense.Models.Towers;
using TowerDefense.Models.WeaponUpgrades;

namespace TowerDefense.Interfaces
{
    public interface IWeapon
    {
        public int GetDamage();

        public int GetRange();

        public void IncreaseDamage(int amount);
        
        public void IncreaseRange(int amount);

        public void IncreaseSpeed(int amount);

        public void SetDamage(int value);

        public void SetRange(int value);

        public void SetSpeed(int value);

        public List<Bullet> Shoot(Tower tower, List<Enemy> enemies, int damage, int numb = 1);
    }
}
