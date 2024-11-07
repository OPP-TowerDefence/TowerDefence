using TowerDefense.Models.TileEffects;
using TowerDefense.Utils;
using TowerDefense.Interfaces;

namespace TowerDefense.Models
{
    public class PathPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
        public ITileEffect Effect { get; set; }

        public PathPoint(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            Type = type;
            Effect = CreateEffectForTileType(type);
        }

        public ITileEffect CreateEffectForTileType(TileType type)
        {
            return type switch
            {
                TileType.Ice => new IceTileEffect(),
                TileType.Mud => new MudTileEffect(),
                TileType.PinkHealth => new PinkTileEffect(),
                _ => null
            };
        }
    }
}