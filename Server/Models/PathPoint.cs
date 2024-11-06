using TowerDefense.Models.TileEffects;
using TowerDefense.Utils;
namespace TowerDefense.Models
{
    public class PathPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }  // Include the type of tile
        public ITileEffect Effect { get; set; }  // Reference to the effect of the tile

        public PathPoint(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            Type = type;
            Effect = CreateEffectForTileType(type);  // Assign the correct effect based on the tile type
        }

        public ITileEffect CreateEffectForTileType(TileType type)
        {
            // Return the appropriate effect based on the tile type
            return type switch
            {
                TileType.Ice => new IceTileEffect(),
                TileType.Mud => new MudTileEffect(),
                TileType.PinkHealth => new PinkTileEffect(),
                _ => null  // No effect for normal tiles or turret tiles
            };
        }
    }
}