using TowerDefense.Interfaces;
using TowerDefense.Models.Tiles;
using TowerDefense.Models.TileEffects;
using TowerDefense.Models.Enemies;
using TowerDefense.Enums;

namespace TowerDefense.Models
{
    public class PathPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
        public ITileEffect Effect { get; set; }
        public IEffectApplicationType EffectApplicationType { get; set; }

        public PathPoint(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            Type = type;
            SetEffectAndApplication(type);
        }

        public void SetEffectAndApplication(TileType type)
        {
            Effect = type switch
            {
                TileType.Ice => new SpeedUpEffect(2, 3),
                TileType.Mud => new SlowDownEffect(2),
                TileType.PinkHealth => new HealEffect(10),
                _ => null
            };

            EffectApplicationType = type switch
            {
                TileType.Ice => new InstantApplication(),
                TileType.Mud => new InstantApplication(),
                TileType.PinkHealth => new DelayedApplication(5),
                _ => new InstantApplication()
            };
        }

        public void ApplyEffect(Enemy enemy)
        {
            if (Effect != null && EffectApplicationType != null)
            {
                EffectApplicationType.ApplyEffect(Effect, enemy);
            }
        }
    }
}