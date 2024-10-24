using Microsoft.AspNetCore.SignalR;
using TowerDefense.Enums;
using TowerDefense.Interfaces;
using TowerDefense.Models.Enemies;
using TowerDefense.Models.WeaponUpgrades;
using TowerDefense.Utils;

namespace TowerDefense.Models;

public class GameState
{
    private const int _commandHistoryLimit = 3;
    private const int _maxHealth = 100;

    private readonly List<TowerTypes> _availableTowerTypes;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly LevelManager _levelManager;
    private readonly Dictionary<string, LinkedList<ICommand>> _playerCommands = [];
    private readonly List<Player> _players;
    private readonly ResourceManager _resourceManager;
    private readonly string _roomCode;

    public bool GameStarted { get; private set; }
    public int Health { get; set; } = _maxHealth;
    public Map Map { get; } = new Map(10, 10);
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

        _levelManager = new LevelManager(new LevelProgressionFacade(this, Map.Enemies, Map.Towers));
        _levelManager.OnLevelChanged += NotifyLevelChange;
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

    public void DecreaseHealth(int amount)
    {
        Health = Math.Max(0, Health - amount);
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
                b.Y
            })
            .ToList();
    }

    public object GetMapEnemies()
    {
        return Map.Enemies
            .Select(e => new
            {
                e.X,
                e.Y,
                e.Health,
                e.Speed
            })
            .ToList();
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

    public void IncreaseHealth(int amount)
    {
        Health = Math.Min(Health + amount, _maxHealth);
    }

    public void ProcessCommand(string connectionId, ICommand command)
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

    public void SpawnEnemies()
    {
        var enemy = GetRandomEnemyFactory()
            .CreateEnemy(0, 0);

        Map.Enemies.Add(enemy);

        _levelManager.OnEnemySpawned();
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
        if (Map.Enemies.Count == 0 || Map.Towers.Count == 0)
        {
            return;
        }

        UpdateBulletPositions();

        foreach (var tower in Map.Towers.ToList())
        {
            var towerBullets = tower.Shoot(Map.Enemies);

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
        foreach (var enemy in Map.Enemies.ToList())
        {
            //enemy.MoveTowardsTarget();

            if (enemy.HasReachedDestination())
            {
                DecreaseHealth(5);
                DamageEnemy(enemy, enemy.Health);
            }
        }
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

    private void DamageEnemy(Enemy enemy, int damage)
    {
        enemy.TakeDamage(damage);

        if (enemy.IsDead())
        {
            Map.Enemies.Remove(enemy);

            _resourceManager.OnEnemyDeath(enemy);
        }
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
        _hubContext.Clients.Group(_roomCode).SendAsync("LevelChanged", newLevel);
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
}
