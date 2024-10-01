using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Timers;
using TowerDefense.Models;

namespace TowerDefense.Services;
public class GameService
{
    private static readonly ConcurrentDictionary<string, GameState> _rooms = new();
    private readonly System.Timers.Timer _gameTickTimer;
    private readonly IHubContext<GameHub> _hubContext;

    public GameService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;

        _gameTickTimer = new System.Timers.Timer(250);
        _gameTickTimer.Elapsed += GameTickHandler;
        _gameTickTimer.AutoReset = true;
        _gameTickTimer.Start();
    }

    public ConcurrentDictionary<string, GameState> Rooms => _rooms;

    private async void GameTickHandler(object sender, ElapsedEventArgs e)
    {
        foreach (var room in _rooms)
        {
            var gameState = room.Value;

            gameState.ProcessTowerPlacements();

            await _hubContext.Clients
                .Group(room.Key)
                .SendAsync("OnTick", gameState.GetMap());
        }
    }
}