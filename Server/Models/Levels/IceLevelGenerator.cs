using System;
using System.Collections.Generic;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Models.Levels;
using TowerDefense.Models.Mediator;

namespace TowerDefense.Models.Levels
{
    public class IceLevelGenerator : LevelGenerationTemplate
    {

        public IceLevelGenerator(Map map) : base(map) {}
        
        protected override List<PathPoint> GeneratePathToObjective()
        {
            var path = new List<PathPoint>();
            var start = Map.GetTile(0, 0);
            int currentX = start.X;
            int currentY = start.Y;

            int mapHeight = Map.Height;
            int mapWidth = Map.Width;

            var objective = Map.GetObjectiveTile();
            if (objective == null)
            {
                throw new InvalidOperationException("Objective tile not set in the map.");
            }

            while (currentY < mapHeight)
            {
                for (int i = 0; i < 20 && currentX < mapWidth; i++)
                {
                    var point = Map.GetTile(currentX, currentY);
                    path.Add(point);
                    currentX++;
                }

                if (currentX >= mapWidth)
                {
                    for (int i = 0; i < 5 && currentY < mapHeight; i++)
                    {
                        var point = Map.GetTile(currentX - 1, currentY);
                        path.Add(point);
                        currentY++;
                    }
                }
                else
                {
                    for (int i = 0; i < 15 && currentY < mapHeight; i++)
                    {
                        var point = Map.GetTile(currentX, currentY);
                        path.Add(point);
                        currentY++;
                    }
                }
                if (currentX >= objective.X && currentY >= objective.Y)
                {
                    break;
                }
            }

            return path;
        }

        protected override void AddSpecialTileSections()
        {
            foreach (var path in Map.Paths)
            {
                int start = path.Count / 2;
                int end = start + (path.Count / 20);

                for (int i = start; i < end; i++)
                {
                    var point = path[i];
                    if (point != null)
                    {
                        Map.SetTileType(point.X, point.Y, TileType.Ice);
                    }
                }
            }
        }
    }
}
