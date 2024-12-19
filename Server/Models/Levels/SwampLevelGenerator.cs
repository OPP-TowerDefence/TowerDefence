using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Models.Levels;
using TowerDefense.Models.Mediator;

namespace TowerDefense.Models.Levels
{
    public class SwampLevelGenerator : LevelGenerationTemplate
    {

        public SwampLevelGenerator(Map map) : base(map) {}

        protected override List<PathPoint> GeneratePathToObjective()
        {
            return GenerateMultiStepPath();
        }

        private List<PathPoint> GenerateMultiStepPath()
        {
            var start = Map.GetTile(0, 0);
            var objective = Map.GetObjectiveTile();
            var path = new List<PathPoint>();

            if (objective == null)
            {
                throw new InvalidOperationException("Objective tile not set in the map.");
            }

            int currentX = start.X, currentY = start.Y;

            while (currentX < Map.Width - 1 && currentY < Map.Height - 1)
            {
                for (int i = 0; i < 5 && currentX < Map.Width - 1; i++)
                {
                    var point = Map.GetTile(currentX, currentY);
                    path.Add(point);
                    currentX++;

                }

                for (int i = 0; i < 5 && currentY < Map.Height - 1; i++)
                {

                    var point = Map.GetTile(currentX, currentY);
                    if (point != null)
                    {
                        path.Add(point);
                    }
                    currentY++;
                }
            }

            while (currentX < objective.X)
            {
                var point = Map.GetTile(currentX, currentY);
                path.Add(point);
                currentX++;
            }

            while (currentY < objective.Y)
            {
                var point = Map.GetTile(currentX, currentY);
                path.Add(point);
                currentY++;
            }
            return path;
        }

        protected override void AddSpecialTileSections()
        {
            foreach (var path in Map.Paths)
            {
                int start = path.Count / 3;
                int end = start + (path.Count / 15);

                for (int i = start; i < end; i++)
                {
                    var point = path[i];
                    if (point != null)
                    {
                        Map.SetTileType(point.X, point.Y, TileType.Mud);
                    }
                }
            }
        }
    }
}
