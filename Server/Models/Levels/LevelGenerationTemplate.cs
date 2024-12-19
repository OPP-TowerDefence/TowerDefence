using TowerDefense.Enums;

namespace TowerDefense.Models.Levels
{
    public abstract class LevelGenerationTemplate
    {
        protected Map Map;
        protected Random _random;
        private static int _pathGenerationCount = 0;

        public LevelGenerationTemplate(Map map)
        {
            Map = map;
            _random = new Random();
        }

        public virtual void GeneratePaths()
        {
            if (_pathGenerationCount < 3)
            {
                var newPath = GeneratePathToObjective();
                Map.Paths.Add(newPath);
                _pathGenerationCount++;
            }
            
            SetAllPathTilesToNormal();
            AddSpecialTileSections();
            EnsureObjectiveTile();
        }

        protected virtual void SetAllPathTilesToNormal()
        {
            foreach (var path in Map.Paths)
            {
                foreach (var point in path)
                {
                    if (point != null && Map.GetTileType(point.X, point.Y) != TileType.Objective)
                    {
                        Map.SetTileType(point.X, point.Y, TileType.Normal);
                    }
                }
            }
        }

        protected abstract List<PathPoint> GeneratePathToObjective();

        protected abstract void AddSpecialTileSections();

        protected virtual void EnsureObjectiveTile()
        {
            var objective = Map.GetObjectiveTile();
            if (objective != null)
            {
                Map.SetTileType(objective.X, objective.Y, TileType.Objective);
            }
        }
    }
}
