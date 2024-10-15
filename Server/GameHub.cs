using Microsoft.AspNetCore.SignalR;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Services;
using TowerDefense.Utils;

namespace TowerDefense;
public class GameHub(GameService gameService, Interfaces.ILogger logger) : Hub
{
    private readonly GameService _gameService = gameService;
    private readonly Interfaces.ILogger _logger = logger;

    public async Task JoinRoom(string roomCode, string username)
    {
        _logger.LogInfo($"Player {username} is attempting to join room {roomCode}.");

        if (!_gameService.Rooms.TryGetValue(roomCode, out var gameState))
        {
            gameState = new GameState(_logger);

            _gameService.Rooms.TryAdd(roomCode, gameState);

            _logger.LogInfo($"New room created with code: {roomCode}.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

        gameState.AddPlayer(username, Context.ConnectionId);

        await Clients.Caller.SendAsync("InitializeMap", gameState.Map.Width, gameState.Map.Height, gameState.GetMapTowers(), gameState.GetMapEnemies());

        _logger.LogInfo($"Player {username} with connection ID {Context.ConnectionId} has joined room {roomCode}.");

        var activeUsernames = gameState.GetActivePlayers();

        await Clients
            .Group(roomCode)
            .SendAsync("UserJoined", username, activeUsernames);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var room in _gameService.Rooms)
        {
            var gameState = room.Value;

            var player = gameState.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

            if (player != null)
            {
                gameState.RemovePlayer(Context.ConnectionId);

                _logger.LogInfo($"Player {player.Username} left room {room.Key}.");

                var activeUsernames = gameState.GetActivePlayers();

                await Clients
                    .Group(room.Key)
                    .SendAsync("UserLeft", player.Username, activeUsernames);
            }
        }

        if (exception != null)
        {
            _logger.LogException(exception);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task PlaceTower(string roomCode, int x, int y, TowerCategories towerCategory)
    {
        if (_gameService.Rooms.TryGetValue(roomCode, out var gameState))
        {
             gameState.QueueTowerPlacement(x, y, Context.ConnectionId, towerCategory);
        }
        else
        {
            _logger.LogError($"Failed to place tower: room code {roomCode} does not exist.");
        }
    }
}
