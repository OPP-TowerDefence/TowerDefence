using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Timers;
using TowerDefense.Models.Collections;
using TowerDefense.Utils;

namespace TowerDefense.Services;
public class GameService
{
    //private static readonly ConcurrentDictionary<string, GameState> _rooms = new();
    private static readonly RoomCollection _rooms = new();
    private readonly System.Timers.Timer _gameTickTimer;
    private readonly IHubContext<GameHub> _hubContext;

    private double _timeSinceLastSpawn = 0;
    private const double SpawnInterval = 5000;

    private const double EnvironmentUpdateInterval = 15000;
    private double _timeSinceLastEnvironmentUpdate = 0;

    public GameService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;

        _gameTickTimer = new System.Timers.Timer(250);
        _gameTickTimer.Elapsed += GameTickHandler;
        _gameTickTimer.AutoReset = true;
        _gameTickTimer.Start();

        Logger.Instance.LogInfo("GameService initialized and game tick timer started.");
    }

    public RoomCollection Rooms => _rooms;

    private async void GameTickHandler(object sender, ElapsedEventArgs e)
    {
        _timeSinceLastSpawn += 250;
        _timeSinceLastEnvironmentUpdate += 250;

        foreach (var room in _rooms.Where(r => r.GameStarted))
        {
            try
            {
                if (_timeSinceLastSpawn >= SpawnInterval)
                {
                    room.SpawnEnemies();
                    _timeSinceLastSpawn = 0;
                }
                
                if (_timeSinceLastEnvironmentUpdate >= EnvironmentUpdateInterval)
                {
                    room.UpdateEnvironment();
                    _timeSinceLastEnvironmentUpdate = 0;
                }

                room.UpdateEnemies();
                room.TowerAttack();

                await _hubContext.Clients
                    .Group(room.RoomCode)
                    .SendAsync("OnTick", room.GetMapTowers(), room.GetMapEnemies(), room.GetMapBullets(), room.SendPath());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }
    }
}