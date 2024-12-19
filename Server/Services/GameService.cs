using Microsoft.AspNetCore.SignalR;
using System.Timers;
using TowerDefense.Models;
using TowerDefense.Models.Collections;
using TowerDefense.Utils;

namespace TowerDefense.Services;
public class GameService
{
    private const double _environmentUpdateInterval = 15000;
    private const double _spawnInterval = 5000;

    private static readonly RoomCollection _rooms = new();
    private readonly System.Timers.Timer _gameTickTimer;
    private readonly IHubContext<GameHub> _hubContext;
    private double _timeSinceLastSpawn = 0;

    public RoomCollection Rooms => _rooms;

    public GameService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;

        _gameTickTimer = new System.Timers.Timer(250);
        _gameTickTimer.Elapsed += GameTickHandler;
        _gameTickTimer.AutoReset = true;
        _gameTickTimer.Start();

        Logger.Instance.LogInfo("GameService initialized and game tick timer started.");
    }


    private async void GameTickHandler(object sender, ElapsedEventArgs e)
    {
        _timeSinceLastSpawn += _gameTickTimer.Interval;

        foreach (var room in _rooms.Where(r => r.GameStarted))
        {
            try
            {
                if (room.Map.MainObject.IsDestroyed())
                {                   
                    await _hubContext.Clients
                        .Group(room.RoomCode)
                        .SendAsync("OnGameOver", room.GetGameOverInfo());

                    DestroyRoom(room.RoomCode, room);

                    continue;
                }

                if (_timeSinceLastSpawn >= _spawnInterval)
                {
                    room.SpawnEnemies();
                    _timeSinceLastSpawn = 0;
                }

                room.ReverseEffects();
                room.ApplyEffects();
                room.GenerateResources();
                room.UpdateEnemies();
                room.TowerAttack();

                await _hubContext.Clients
                    .Group(room.RoomCode)
                    .SendAsync("OnTick", room.GetMapTowers(), room.GetMapEnemies(), room.GetMapBullets(), room.SendPath(), room.GetMainObject());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }
    }

    private void DestroyRoom(string roomCode, GameState? gameState)
    {
        _rooms.Remove(roomCode, out gameState);
    }
}