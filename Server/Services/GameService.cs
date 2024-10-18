using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Timers;
using TowerDefense.Models;
using TowerDefense.Utils;

namespace TowerDefense.Services;
public class GameService
{
    private static readonly ConcurrentDictionary<string, GameState> _rooms = new();
    private readonly System.Timers.Timer _gameTickTimer;
    private readonly IHubContext<GameHub> _hubContext;

    private double _timeSinceLastSpawn = 0;
    private const double SpawnInterval = 3000;

    public GameService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;

        _gameTickTimer = new System.Timers.Timer(250);
        _gameTickTimer.Elapsed += GameTickHandler;
        _gameTickTimer.AutoReset = true;
        _gameTickTimer.Start();

        Logger.Instance.LogInfo("GameService initialized and game tick timer started.");
    }

    public ConcurrentDictionary<string, GameState> Rooms => _rooms;

   private async void GameTickHandler(object sender, ElapsedEventArgs e)
{
    foreach (var room in _rooms)
    {
        var gameState = room.Value;

        try
        {
            // Process tower placements in the room
            gameState.ProcessTowerPlacements();

            // Increment the time since the last enemy spawn for this specific room
            gameState.TimeSinceLastSpawn += 250; // Add 250ms for each tick

            // Spawn enemies if the time since the last spawn exceeds the spawn interval
            if (gameState.TimeSinceLastSpawn >= SpawnInterval)
            {
                gameState.SpawnEnemies(); // Spawn enemies in the room
                gameState.TimeSinceLastSpawn = 0;  // Reset spawn timer for this room
            }

            // Update enemy positions and other game logic
            gameState.UpdateEnemies();

            // Send the updated game state (towers, enemies, paths) to all clients in the room
            await _hubContext.Clients
                .Group(room.Key)
                .SendAsync("OnTick", gameState.GetMapTowers(), gameState.GetMapEnemies(), gameState.SendPath());
        }
        catch (Exception ex)
        {
            Logger.Instance.LogException(ex);
        }
    }
}
}