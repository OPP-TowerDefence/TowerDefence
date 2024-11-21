using Microsoft.AspNetCore.SignalR;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Commands;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Utils;
using TowerDefense.Models.Strategies;
using TowerDefense.Models.TileEffects;

namespace TowerDefense.Models;

public class GameState
{
    private const int _commandHistoryLimit = 3;
    private const int _baseEnemiesPerLevel = 10;

    private readonly List<TowerTypes> _availableTowerTypes;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly Dictionary<string, LinkedList<ICommand>> _playerCommands = [];
    private readonly List<Player> _players;
    private readonly ResourceManager _resourceManager;
    private readonly LevelProgressionFacade _levelFacade;
    private readonly string _roomCode;

    private int _enemiesSpawned = 0;
    private int _currentLevel = 1;
    private Random _random = new Random();

    public int EnemyCount { get; private set; } = 0;
    public bool GameStarted { get; private set; }
    public event Action<int>? OnLevelChanged;

    public Map Map { get; } = new Map(100, 100);
    public List<Player> Players => _players;

    public GameState(IHubContext<GameHub> hubContext, string roomCode)
    {
        _hubContext = hubContext;
        _players = [];
        _resourceManager = new();
        _roomCode = roomCode;

        _availableTowerTypes = Enum
            .GetValues(typeof(TowerTypes))
            .Cast<TowerTypes>()
            .ToList();

        _levelFacade = new LevelProgressionFacade(Map.MainObject, GetAllEnemies(Map.Enemies).ToList(), Map.Towers);
        OnLevelChanged += NotifyLevelChange;
    }

    public void AddPlayer(string username, string connectionId)
    {
        if (!_players.Any(p => p.ConnectionId == connectionId))
        {
            if (_availableTowerTypes.Count > 0)
            {
                var playerTowerType = _availableTowerTypes.First();

                var player = new Player(username, connectionId, playerTowerType, _hubContext);

                _players.Add(player);
                _resourceManager.Attach(player);

                _availableTowerTypes.Remove(playerTowerType);

                Logger.Instance.LogInfo($"Player {username} joined the game.");
            }
            else
            {
                Logger.Instance.LogError($"Player {username} could not be added. No available tower types left to assign to a new player.");
            }
        }
    }

    public IEnumerable<object> GetActivePlayers()
    {
        return _players
            .Select(p => new
            {
                Username = p.Username,
                TowerType = p.TowerType.ToString()
            })
            .ToList();
    }

    public object GetMapBullets()
    {
        return Map.Bullets
            .Select(b => new
            {
                b.X,
                b.Y,
                b.Flyweight.Path
            })
            .ToList();
    }

    public object GetMapEnemies()
    {
        var allEnemies = GetAllEnemies(Map.Enemies);

        return allEnemies
            .Select(e => new
            {
                e.X,
                e.Y,
                e.Health,
                e.Speed,
                Type = e.Type.ToString()
            })
            .ToList();
    }

    private IEnumerable<Enemy> GetAllEnemies(IEnumerable<IEnemyComponent> components)
    {
        foreach (var component in components)
        {
            if (component is Enemy enemy)
            {
                yield return enemy;
            }
            else if (component is EnemyGroup group)
            {
                foreach (var subEnemy in GetAllEnemies(group.Children))
                {
                    yield return subEnemy;
                }
            }
        }
    }

    public object GetMapTowers()
    {
        return Map.Towers
            .Select(t => new
            {
                t.X,
                t.Y,
                Category = t.Category.ToString(),
                Type = t.Type.ToString(),
                AppliedUpgrades = t.AppliedUpgrades.Select(u => u.ToString()).ToList()
            })
            .ToList();
    }

    public void ProcessCommand(ICommand command, string connectionId)
    {
        var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);

        if (!_playerCommands.ContainsKey(connectionId))
        {
            _playerCommands[connectionId] = new();
        }

        var commands = _playerCommands[connectionId];

        if (commands.Count >= _commandHistoryLimit)
        {
            commands.RemoveLast();
        }

        commands.AddFirst(command);

        command.Execute();
    }

    public void RemovePlayer(string connectionId)
    {
        var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);

        if (player is not null)
        {
            _availableTowerTypes.Add(player.TowerType);

            _players.Remove(player);
            _resourceManager.Detach(player);

            Logger.Instance.LogInfo($"Player {player.Username} left the game.");
        }
        else
        {
            Logger.Instance.LogError($"Could not remove player with connection ID {connectionId}. Player was not found.");
        }
    }

    public object SendPath()
    {
        return Map.Paths.Select(path => path.Select(tile => new
        {
            X = tile.X,
            Y = tile.Y,
            Type = tile.Type.ToString()
        })).ToList();
    }

    public void SpawnEnemies()
    {
        int spawnType = DetermineSpawnType(_currentLevel);

        IEnemyComponent enemyComponent;

        if (spawnType == 0)
        {
            Console.WriteLine("Spawning a single enemy.");
            enemyComponent = CreateAndInitializeEnemy(0, 0);
        }
        else
        {
            int numberOfEnemies = spawnType == 1 ? 2 : 3;
            Console.WriteLine($"Spawning a group of {numberOfEnemies} enemies.");
            var enemyGroup = new EnemyGroup();

            var spawnPositions = GetGroupOffsets(numberOfEnemies);

            foreach (var (offsetX, offsetY) in spawnPositions)
            {
                var enemy = CreateAndInitializeEnemy(offsetX, offsetY);
                enemyGroup.Add(enemy);
                Console.WriteLine($"HP: {enemy.Health}");
            }

            enemyComponent = enemyGroup;
        }

        Map.Enemies.Add(enemyComponent);
    }

    private int DetermineSpawnType(int currentLevel)
    {
        if (currentLevel == 1)
        {
            return 0;
        }
        else if (currentLevel == 2)
        {
            return _random.Next(0, 2);
        }
        else
        {
            return _random.Next(0, 3);
        }
    }

    private Enemy CreateAndInitializeEnemy(int offsetX, int offsetY)
    {
        var enemy = GetRandomEnemyFactory().CreateEnemy(offsetX, offsetY);
        enemy.RetrievePath(this);
        _levelFacade.ApplyBuffToNewEnemy(enemy);
        OnEnemySpawned();

        if (_enemiesSpawned % 30 == 0)
        {
            enemy.MarkAsShadowEnemy();
        }

        if (enemy is StrongEnemy strongEnemy && !enemy.IsShadowEnemy)
        {
            NotifyShadowEnemies(strongEnemy);
        }

        return enemy;
    }

    private List<(int X, int Y)> GetGroupOffsets(int groupSize)
    {
        var offsets = new List<(int X, int Y)>();
        int cols = (int)Math.Ceiling(Math.Sqrt(groupSize));
        int rows = (int)Math.Ceiling((double)groupSize / cols);

        for (int i = 0; i < groupSize; i++)
        {
            int offsetX = (i % cols);
            int offsetY = (i / cols);
            offsets.Add((offsetX, offsetY));
        }

        return offsets;
    }

    private void OnEnemySpawned()
    {
        _enemiesSpawned++;

        int enemiesRequiredForNextLevel = _baseEnemiesPerLevel * _currentLevel * (_currentLevel + 1) / 2;

        if (_enemiesSpawned >= enemiesRequiredForNextLevel)
        {
            _levelFacade.IncreaseLevel();
            _currentLevel = _levelFacade.GetCurrentLevel();

            OnLevelChanged?.Invoke(_currentLevel);
        }
    }

    private void NotifyShadowEnemies(StrongEnemy newStrongEnemy)
    {
        foreach (var shadowEnemy in Map.Enemies.OfType<Enemy>().Where(e => e.CurrentStrategy is ShadowStrategy))
        {
            var shadowStrategy = (ShadowStrategy)shadowEnemy.CurrentStrategy;

            if (!shadowStrategy.HasTarget())
            {
                shadowStrategy.SetTargetStrongEnemy(newStrongEnemy);
                shadowEnemy.RetrievePath(this);
            }
        }
    }

    public void RecalculatePathsForStrategy(IPathStrategy strategy)
    {
        var allEnemies = GetAllEnemies(Map.Enemies);

        foreach (var enemy in allEnemies)
        {
            if (enemy.CurrentStrategy == strategy)
            {
                enemy.RetrievePath(this);
            }
        }
    }

    public void StartGame(string connectionId)
    {
        if (!_players.Any(p => string.Equals(p.ConnectionId, connectionId)))
        {
            throw new Exception($"Game could not be started. The user with connection ID {connectionId} is not in the game.");
        }
        GameStarted = true;
    }

    public void PlaceTower(int x, int y, TowerCategories towerCategory, Player player)
    {
        var tower = player.CreateTower(x, y, towerCategory);
        var command = new PlaceTowerCommand(this.Map, tower, _levelFacade);
        ProcessCommand(command, player.ConnectionId);
        Map.UpdateDefenseMap();
        RecalculatePathsForStrategy(new ThreatAvoidanceStrategy());
    }

    public void TowerAttack()
    {
        if (Map.Enemies.Count == 0 || Map.Towers.Count == 0)
        {
            return;
        }

        UpdateBulletPositions();

        var allEnemies = GetAllEnemies(Map.Enemies);

        foreach (var tower in Map.Towers.ToList())
        {
            var towerBullets = tower.Shoot(allEnemies.ToList());

            if (towerBullets.Count == 0)
            {
                continue;
            }

            Map.Bullets.AddRange(towerBullets);
        }
    }

    public ICommand? UndoLastCommand(string connectionId)
    {
        if (_playerCommands.TryGetValue(connectionId, out var commands) && commands.Any())
        {
            var command = commands.First();

            command.Undo();

            commands.RemoveFirst();

            return command;
        }

        return default;
    }

    public void UpdateEnemies()
    {
        foreach (var enemyComponent in Map.Enemies.ToList())
        {
            enemyComponent.MoveTowardsNextWaypoint(this);

            if (enemyComponent.HasReachedDestination())
            {
                enemyComponent.HandleDestination(Map.MainObject, this);
            }

            if (enemyComponent.IsDead())
            {
                Map.Enemies.Remove(enemyComponent);
            }
        }
    }

    public void UpdateEnvironment()
    {
        var objective = Map.GetObjectiveTile();

        for (int x = 0; x < Map.Width; x++)
        {
            for (int y = 0; y < Map.Height; y++)
            {
                var tile = Map.GetTile(x, y);

                if (tile.Type == TileType.Turret || (x == objective.X && y == objective.Y))
                    continue;

                tile.Type = TileType.Normal;
                tile.Effect = null;
                tile.EffectApplicationType = null;
            }
        }

        for (int x = 0; x < Map.Width; x++)
        {
            for (int y = 0; y < Map.Height; y++)
            {
                var tile = Map.GetTile(x, y);

                if (tile.Type == TileType.Normal && _random.NextDouble() < 0.9)
                {
                    var newTileType = Map.DetermineTileType();
                    tile.Type = newTileType;
                    tile.SetEffectAndApplication(newTileType);
                }
            }
        }

        RecalculatePathsForStrategy(new SpeedPrioritizationStrategy());
        RecalculatePathsForStrategy(new SurvivalStrategy());
    }
    public void UpgradeTower(int x, int y, TowerUpgrades towerUpgrade)
    {
        var tower = Map.Towers.FirstOrDefault(t => t.X == x && t.Y == y);

        if (tower is null)
        {
            Logger.Instance.LogError($"Unable to upgrade tower at position ({x},{y}). Tower not found.");

            return;
        }

        if (tower.AppliedUpgrades.Contains(towerUpgrade))
        {
            Logger.Instance.LogError($"Tower at position ({x},{y}) already has upgrade {towerUpgrade}.");

            return;
        }

        tower.Weapon = towerUpgrade switch
        {
            TowerUpgrades.Burst => new Burst(tower.Weapon),
            TowerUpgrades.DoubleDamage => new DoubleDamage(tower.Weapon),
            TowerUpgrades.DoubleBullet => new DoubleBullet(tower.Weapon),
            _ => throw new Exception("Unknown tower upgrade")
        };

        tower.AppliedUpgrades.Add(towerUpgrade);
    }

    public void HandleEnemyDeath(Enemy enemy)
    {
        Map.Enemies.Remove(enemy);

        _resourceManager.OnEnemyDeath(enemy);

        Console.WriteLine($"Enemy {enemy} has died and resources updated.");
    }

    private void DamageEnemy(IEnemyComponent enemyComponent, int damage)
    {
        enemyComponent.TakeDamage(damage, this);
    }

    private IEnemyFactory GetRandomEnemyFactory()
    {
        Random rand = new();

        int enemyType = rand.Next(0, 3);

        switch (enemyType)
        {
            case 0:
                return new FastEnemyFactory();
            case 1:
                return new StrongEnemyFactory();
            case 2:
                return new FlyingEnemyFactory();
            default:
                Logger.Instance.LogError($"Unknown enemy type {enemyType} generated in RandomEnemyFactory.");
                throw new Exception("Unknown enemy type");
        }
    }

    private void NotifyLevelChange(int newLevel)
    {
        _hubContext.Clients
            .Group(_roomCode)
            .SendAsync("LevelChanged", newLevel);
    }

    private void UpdateBulletPositions()
    {
        if (Map.Bullets.Count == 0)
        {
            return;
        }

        var bullets = Map.Bullets.ToList();
        var allEnemies = GetAllEnemies(Map.Enemies);

        foreach (var bullet in bullets)
        {
            var enemyToAttack = allEnemies.FirstOrDefault(e => e.Id == bullet.EnemyId);

            if (enemyToAttack is not null)
            {
                bullet.Move(enemyToAttack.X, enemyToAttack.Y);

                if (bullet.X == enemyToAttack.X && bullet.Y == enemyToAttack.Y)
                {
                    DamageEnemy(enemyToAttack, bullet.Damage);

                    Map.Bullets.Remove(bullet);
                }
            }
            else
            {
                Map.Bullets.Remove(bullet);
            }
        }
    }
}
