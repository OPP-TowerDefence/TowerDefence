using TowerDefense.Enums;

namespace TowerDefense.Models.Levels
{
    public class RainbowLevelGenerator : LevelGenerationTemplate
    {
        public RainbowLevelGenerator(Map map) : base(map) { }

        protected override List<PathPoint> GeneratePathToObjective()
        {
            return GenerateRandomPath();
        }

        private List<PathPoint> GenerateRandomPath()
        {
            var path = new List<PathPoint>();
            var start = Map.GetTile(0, 0);
            var objective = Map.GetObjectiveTile();
            int currentX = start.X, currentY = start.Y;

            while (currentX != objective.X || currentY != objective.Y)
            {
                bool moveHorizontally = _random.Next(0, 2) == 0;

                if (moveHorizontally && currentX != objective.X)
                    currentX += (currentX < objective.X) ? 1 : -1;
                else if (currentY != objective.Y)
                    currentY += (currentY < objective.Y) ? 1 : -1;

                var point = Map.GetTile(currentX, currentY);
                path.Add(point);
            }

            return path;
        }

        protected override void AddSpecialTileSections()
        {
            foreach (var path in Map.Paths)
            {
                int start = path.Count / 3;
                int end = 2 * path.Count / 3;

                for (int i = start; i < end; i += 5)
                {
                    var point = path[i];
                    if (point != null)
                    {
                        Map.SetTileType(point.X, point.Y, TileType.PinkHealth);
                    }
                }
            }
        }
    }
}