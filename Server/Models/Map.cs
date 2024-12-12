using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Enums;
using TowerDefense.Models.Collections;
using TowerDefense.Models.Mediator;

namespace TowerDefense.Models
{
    public class Map
    {
        private readonly Dictionary<(int x, int y), int> _defenseMap = [];
        private readonly Random _random = new();

        private PathPoint[,] _tiles;

        public int Height { get; private set; }
        public int Width { get; private set; }
        public EnemyCollection Enemies { get; set; } = [];
        public List<Bullet> Bullets { get; set; } = [];
        public List<List<PathPoint>> Paths { get; private set; } = [];
        public TowerManager TowerManager;
        public MainObject MainObject { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            InitializeTiles();
            PlaceObjectiveTile();
            GenerateRandomPaths();
        }

        public void UpdateDefenseMap()
        {
            _defenseMap.Clear();
            const int maxRange = 10;

            foreach (var turret in TowerManager.Towers)
            {
                for (int dx = -maxRange; dx <= maxRange; dx++)
                {
                    for (int dy = -maxRange; dy <= maxRange; dy++)
                    {
                        int x = turret.X + dx;
                        int y = turret.Y + dy;

                        if (IsValidPosition(x, y) && GetTileType(x, y ) != TileType.Turret)
                        {
                            int distance = Math.Abs(dx) + Math.Abs(dy);
                            int defenseValue = maxRange - distance + 1;

                            if (_defenseMap.ContainsKey((x, y)))
                            {
                                _defenseMap[(x, y)] += defenseValue;
                            }
                            else
                            {
                                _defenseMap[(x, y)] = defenseValue;
                            }
                        }
                    }
                }
            }
        }

        public int GetDefenseLevel(int x, int y)
        {
            return _defenseMap.TryGetValue((x, y), out int level) ? level : 0;
        }

        private void InitializeTiles()
        {
            _tiles = new PathPoint[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _tiles[x, y] = new PathPoint(x, y, TileType.Turret);
                }
            }
        }

        public void SetTileType(int x, int y, TileType newTileType)
        {
            if (IsValidPosition(x, y))
            {
                _tiles[x, y].Type = newTileType;
                _tiles[x, y].SetEffectAndApplication(newTileType);
            }
        }

        public TileType GetTileType(int x, int y)
        {
            return IsValidPosition(x, y) ? _tiles[x, y].Type : TileType.Turret;
        }

        public PathPoint GetTile(int x, int y)
        {
            return IsValidPosition(x, y) ? _tiles[x, y] : null;
        }

        public List<PathPoint> GetAllTilesOfType(TileType type)
        {
            var tilesOfType = new List<PathPoint>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_tiles[x, y].Type == type)
                    {
                        tilesOfType.Add(_tiles[x, y]);
                    }
                }
            }
            return tilesOfType;
        }

        private void PlaceObjectiveTile()
        {
            int objectiveX = Width - 1;
            int objectiveY = Height - 1;

            SetTileType(objectiveX, objectiveY, TileType.Objective);

            MainObject = new MainObject(objectiveX, objectiveY);
        }

        public void GenerateRandomPaths()
        {
            Paths.Clear();

            int startX = 0;
            int startY = 0;
            
            var objective = GetObjectiveTile();
            
            int sharedPathLength = Math.Min(10, Math.Abs(objective.X - startX) + Math.Abs(objective.Y - startY));
            
            var sharedPath = new List<PathPoint> { GetTile(startX, startY) };
            
            SetTileType(startX, startY, DetermineTileType());

            int currentX = startX;
            int currentY = startY;

            for (int i = 1; i < sharedPathLength && (currentX != objective.X || currentY != objective.Y); i++)
            {
                if (currentX < objective.X)
                {
                    currentX++;
                }
                else if (currentY < objective.Y)
                {
                    currentY++;
                }

                var sharedPoint = GetTile(currentX, currentY);

                if (sharedPoint != null)
                {
                    SetTileType(currentX, currentY, DetermineTileType());

                    sharedPath.Add(sharedPoint);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var newPath = new List<PathPoint>(sharedPath);
                var randomPath = GenerateRandomPathToObjective(newPath.Last().X, newPath.Last().Y, objective.X, objective.Y);
                
                foreach (var pathPoint in randomPath)
                {
                    SetTileType(pathPoint.X, pathPoint.Y, DetermineTileType());
                }

                newPath.AddRange(randomPath);

                if (newPath.Last() != objective)
                {
                    newPath.Add(objective);
                }

                SetTileType(objective.X, objective.Y, TileType.Objective);
                Paths.Add(newPath);
            }
        }

        private List<PathPoint> GenerateRandomPathToObjective(int startX, int startY, int endX, int endY)
        {
            var pathSegment = new List<PathPoint>();
            int currentX = startX;
            int currentY = startY;

            while (currentX != endX || currentY != endY)
            {
                bool moveHorizontally = _random.Next(0, 2) == 0;

                if (moveHorizontally && currentX != endX)
                {
                    currentX += (currentX < endX) ? 1 : -1;
                }
                else if (currentY != endY)
                {
                    currentY += (currentY < endY) ? 1 : -1;
                }

                PathPoint nextPoint = GetTile(currentX, currentY);

                if (nextPoint != null && !pathSegment.Contains(nextPoint))
                {
                    pathSegment.Add(nextPoint);
                }
            }

            return pathSegment;
        }

        public PathPoint GetObjectiveTile()
        {
            return _tiles[Width - 1, Height - 1];
        }

        public TileType DetermineTileType()
        {
            double randomValue = _random.NextDouble();

            if (randomValue < 0.01)
                return TileType.PinkHealth;
            else if (randomValue < 0.10)
                return TileType.Ice;
            else if (randomValue < 0.15)
                return TileType.Mud;

            return TileType.Normal;
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
