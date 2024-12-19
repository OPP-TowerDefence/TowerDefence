using Microsoft.AspNetCore.SignalR;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Commands;
using TowerDefense.Models.Enemies;
using TowerDefense.Utils;
using TowerDefense.Models.Strategies;
using TowerDefense.Interfaces.Visitor;
using TowerDefense.Visitors;
using TowerDefense.Models.Mediator;
using TowerDefense.Models.Mementos;
using TowerDefense.Models.Collections;
using TowerDefense.Models.MainObjectStates;
using TowerDefense.Models.Levels;

namespace TowerDefense.Models;

public class GameState
{
    private const int _commandHistoryLimit = 3;
    private const int _baseEnemiesPerLevel = 10;

    private readonly List<TowerTypes> _availableTowerTypes;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly LevelProgressionFacade _levelFacade;
    private readonly Dictionary<string, LinkedList<ICommand>> _playerCommands = [];
    private readonly List<Player> _players;
    private readonly Random _random = new();
    private ResourceManager _resourceManager;
    private AchievementManager _achievementManager;
    private readonly IMediator _resourceFlowMediator;

    private int _currentLevel = 1;
    private int _enemiesSpawned = 0;
    private event Action<int>? _onLevelChanged;
    private Guid _accessToken = Guid.NewGuid();

    public Queue<Effect> EffectsToApply { get; set; } = [];

    public IList<Effect> EffectsToReverse { get; set; } = [];

    public int EnemyCount { get; private set; } = 0;
    public bool GameStarted { get; private set; }
    public Map Map { get; set; } = new Map(75, 75);
    public List<Player> Players => _players;
    public string RoomCode { get; private set; }
    public int TimeSinceLastSpawn { get; set; } = 0;
    public int TimeSinceLastEnvironmentUpdate { get; set; } = 0;
    private readonly List<Func<Map, LevelGenerationTemplate>> _levelGenerators;

    public GameState(IHubContext<GameHub> hubContext, string roomCode)
    {
        _hubContext = hubContext;
        _players = [];

        Map.TowerManager = new TowerManager(null);
        _achievementManager = new AchievementManager(null);
        _resourceManager = new ResourceManager(null);

        _resourceFlowMediator = new ResourceFlowMediator(Map.TowerManager, _achievementManager, _resourceManager, this);

        _resourceManager.SetMediator(_resourceFlowMediator);
        Map.TowerManager.SetMediator(_resourceFlowMediator);
        _achievementManager.SetMediator(_resourceFlowMediator);

        _availableTowerTypes = Enum
            .GetValues(typeof(TowerTypes))
            .Cast<TowerTypes>()
            .ToList();

        _levelFacade = new LevelProgressionFacade(Map.MainObject, Map.Enemies.ToList(), Map.TowerManager.Towers);

        _onLevelChanged += NotifyLevelChange;
        RoomCode = roomCode;

        _levelGenerators = new List<Func<Map, LevelGenerationTemplate>>
            {
                map => new RainbowLevelGenerator(map),
                map => new SwampLevelGenerator(map),
                map => new IceLevelGenerator(map)
            };
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

    public bool ApplyEffects()
    {
        if (EffectsToApply.Count == 0)
        {
            return false;
        }

        while (EffectsToApply.Count > 0)
        {
            var effect = EffectsToApply.Dequeue();

            ApplyEffect(effect.Applicator);

            NotifyEffectApplied(effect.Applicator);
            Logger.Instance.LogInfo($"Effect {effect.Applicator} applied.");

            EffectsToReverse.Add(effect);
        }

        return true;
    }

    public void ReverseEffects()
    {
        if (EffectsToReverse.Count == 0)
        {
            return;
        }

        var effectsEnded = new List<Effect>();

        foreach (var effect in EffectsToReverse)
        {
            effect.TicksToEnd--;

            if (effect.TicksToEnd <= 0)
            {
                ApplyEffect(effect.Reverser);

                NotifyEffectEnded(effect.Applicator);
                effectsEnded.Add(effect);

                Logger.Instance.LogInfo($"Effect {effect.Applicator} has ended.");
            }
        }

        EffectsToReverse = EffectsToReverse
            .Except(effectsEnded)
            .ToList();
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

    public object GetMainObject()
    {
        MainObject mainObject = Map.MainObject;

        return new
        {
            mainObject.X,
            mainObject.Y,
            mainObject.Health,
            IsDestroyed = mainObject.IsDestroyed(),
            Path = mainObject.GetStateGif()
        };
    }

    public object GetMapEnemies()
    {
        return Map.Enemies
            .Select(e => new
            {
                e.X,
                e.Y,
                e.Health,
                e.Speed,
                Type = e.Type.ToString(),
                e.Flyweight.FileName
            })
            .ToList();
    }

    public object GetMapTowers()
    {
        return Map.TowerManager.Towers
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

    public object GetGameOverInfo()
    {
        return new
        {
            Path = Map.MainObject.GetStateGif(),
            Health = Map.MainObject.Health,
            Level = _currentLevel,
            Resources = _resourceManager.GetResources()
        };
    }

    public void HandleEnemyDeath(Enemy enemy)
    {
        Map.Enemies.Remove(enemy);

        _resourceManager.OnEnemyDeath(enemy);
    }

    public void PlaceTower(int x, int y, TowerCategories towerCategory, Player player)
    {
        if (!_resourceManager.CanAfford(player.PlayerTowerCost(towerCategory)))
        {
            DisplayMessage("Not enough resources to place tower.");
            return;
        }

        var tower = Map.TowerManager.BuyTower(x, y, towerCategory, player);

        if (tower == null) return;

        var command = new PlaceTowerCommand(this.Map, tower, _levelFacade);
        ProcessCommand(command, player.ConnectionId);
        Map.UpdateDefenseMap();
        RecalculatePathsForStrategy(new ThreatAvoidanceStrategy());
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

    public void RecalculatePathsForStrategy(IPathStrategy strategy)
    {
        foreach (var enemy in Map.Enemies)
        {
            if (enemy.CurrentStrategy == strategy)
            {
                enemy.RetrievePath(this);
            }
        }
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

    public void RestoreState(GameStateMemento memento)
    {
        var (currentLevel, towers, enemies, resources, gameMap, mainObjectHealth, mainObjectState) = memento.GetState(_accessToken);

        _currentLevel = currentLevel;
        _levelFacade.SetCurrentLevel(_currentLevel);
        _enemiesSpawned = 0;

        NotifyLevelChange(_currentLevel);

        Map.TowerManager.SetTowers(towers);
        Map.Enemies = new EnemyCollection(enemies);
        _resourceManager.SetResources(resources);
        Map = gameMap;

        Map.MainObject.Health = mainObjectHealth;
        RestoreMainObjectState(Map.MainObject, mainObjectState);
    }


    public GameStateMemento SaveState()
    {
        string mainObjectStateUrl = Map.MainObject.GetStateGif();
        string mainObjectStateName = ExtractStateName(mainObjectStateUrl);

        return new GameStateMemento(
            _accessToken,
            _levelFacade.GetCurrentLevel(),
            Map.TowerManager.Towers.ToList(),
            Map.Enemies.Components.ToList(),
            _resourceManager.GetResources(),
            Map,
            Map.MainObject.Health,
            mainObjectStateName
        );
    }

    public object SendPath()
    {
        return Map.Paths
            .Where(path => path != null)
            .Select(path => path
                .Where(tile => tile != null)
                .Select(tile => new
                {
                    X = tile.X,
                    Y = tile.Y,
                    Type = tile.Type.ToString()
                })
            ).ToList();
    }

    public void SpawnEnemies()
    {
        int spawnType = DetermineSpawnType(_currentLevel);

        IEnemyComponent enemyComponent;

        if (spawnType == 0)
        {
            enemyComponent = CreateAndInitializeEnemy(0, 0);
        }
        else
        {
            int numberOfEnemies = spawnType == 1 ? 2 : 3;
            var enemyGroup = new EnemyGroup();

            var spawnPositions = GetGroupOffsets(numberOfEnemies);

            foreach (var (offsetX, offsetY) in spawnPositions)
            {
                enemyGroup.Add(CreateAndInitializeEnemy(offsetX, offsetY));
            }

            enemyComponent = enemyGroup;
        }

        Map.Enemies.Add(enemyComponent);
    }

    public void StartGame(string connectionId)
    {
        if (!_players.Any(p => string.Equals(p.ConnectionId, connectionId)))
        {
            throw new Exception($"Game could not be started. The user with connection ID {connectionId} is not in the game.");
        }
        GameStarted = true;
    }

    public void TowerAttack()
    {
        if (Map.Enemies.Count == 0 || Map.TowerManager.Towers.Count == 0)
        {
            return;
        }

        UpdateBulletPositions();

        foreach (var tower in Map.TowerManager.Towers.ToList())
        {
            var towerBullets = tower.Shoot(Map.Enemies.ToList());

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
        foreach (var enemyComponent in Map.Enemies.Components.ToList())
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
    public void UpgradeTower(int x, int y, TowerUpgrades towerUpgrade)
    {
        Map.TowerManager.UpgradeTower(x, y, towerUpgrade, _resourceManager, _levelFacade);
    }

    private void ApplyEffect(IEffectVisitor effectVisitor)
    {
        foreach (var enemy in Map.Enemies)
        {
            enemy.Accept(effectVisitor);
        }

        foreach (var tower in Map.TowerManager.Towers)
        {
            tower.Accept(effectVisitor);
        }

        Map.MainObject.Accept(effectVisitor);
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

    private void DamageEnemy(IEnemyComponent enemyComponent, int damage)
    {
        enemyComponent.TakeDamage(damage, this);
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

    private string ExtractStateName(string stateUrl)
    {
        return stateUrl.Split('/').Last().Replace(".gif", "");
    }

    private List<(int X, int Y)> GetGroupOffsets(int groupSize)
    {
        var offsets = new List<(int X, int Y)>();
        int cols = (int)Math.Ceiling(Math.Sqrt(groupSize));
        int rows = (int)Math.Ceiling((double)groupSize / cols);

        for (int i = 0; i < groupSize; i++)
        {
            offsets.Add((i % cols, i / cols));
        }

        return offsets;
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
            .Group(RoomCode)
            .SendAsync("LevelChanged", newLevel);
    }

    private void NotifyEffectApplied(IEffectVisitor effect)
    {
        var messageObjects = new List<object>
        {
            effect.EffectType.ToString()
        };

        if (effect is EnvironmentalHazardVisitor environmentalHazard)
        {
            messageObjects.Add(environmentalHazard.EnvironmentType.ToString());
        }

        _hubContext.Clients
            .Group(RoomCode)
            .SendAsync("EffectApplied", messageObjects);
    }

    private void NotifyEffectEnded(IEffectVisitor effect)
    {
        var messageObjects = new List<object>
        {
            effect.EffectType.ToString()
        };

        if (effect is EnvironmentalHazardVisitor environmentalHazard)
        {
            messageObjects.Add(environmentalHazard.EnvironmentType.ToString());
        }

        _hubContext.Clients
            .Group(RoomCode)
            .SendAsync("EffectEnded", messageObjects);
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

    private void OnEnemySpawned()
    {
        _enemiesSpawned++;

        int enemiesRequiredForNextLevel = _baseEnemiesPerLevel * _currentLevel * (_currentLevel + 1) / 2;

        if (_enemiesSpawned >= enemiesRequiredForNextLevel)
        {
            _levelFacade.IncreaseLevel();
            _currentLevel = _levelFacade.GetCurrentLevel();

            var effectsToApply = EffectResolver.GetEffects(_currentLevel);

            foreach (var effect in effectsToApply)
            {
                EffectsToApply.Enqueue(effect);
            }

            _onLevelChanged?.Invoke(_currentLevel);
            int generatorIndex = (_currentLevel - 1) % _levelGenerators.Count;
            var currentLevelGenerator = _levelGenerators[generatorIndex](Map);
            currentLevelGenerator.GeneratePaths();
        }
    }

    private void RestoreMainObjectState(MainObject mainObject, string stateName)
    {
        switch (stateName.ToLower())
        {
            case "normal":
                mainObject.ChangeState(new NormalState());
                break;
            case "damaged":
                mainObject.ChangeState(new DamagedState());
                break;
            case "critical":
                mainObject.ChangeState(new CriticalState());
                break;
            case "destroyed":
                mainObject.ChangeState(new DestroyedState());
                break;
            default:
                Logger.Instance.LogError($"Unknown MainObject state: {stateName}");
                mainObject.ChangeState(new NormalState());
                break;
        }
    }

    private void UpdateBulletPositions()
    {
        if (Map.Bullets.Count == 0)
        {
            return;
        }

        var bullets = Map.Bullets.ToList();

        foreach (var bullet in bullets)
        {
            var enemyToAttack = Map.Enemies.FirstOrDefault(e => e.Id == bullet.EnemyId);

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

    public void GenerateResources()
    {
        _resourceManager.OnMainObjectGenerated(Map.MainObject);
    }

    public void DisplayMessage(string message)
    {
        _hubContext.Clients
            .Group(RoomCode)
            .SendAsync("DisplayMessage", message);
    }
}