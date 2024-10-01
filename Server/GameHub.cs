using Microsoft.AspNetCore.SignalR;
using TowerDefense.Models;
using TowerDefense.Services;

namespace TowerDefense;
public class GameHub(GameService gameService) : Hub
{
    private readonly GameService _gameService = gameService;

    public async Task JoinRoom(string roomCode, string username)
    {
        if (!_gameService.Rooms.TryGetValue(roomCode, out var gameState))
        {
            gameState = new GameState();
            _gameService.Rooms.TryAdd(roomCode, gameState);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

        await Clients.Caller.SendAsync("InitializeMap", gameState.Map.MapWidth, gameState.Map.MapHeight, gameState.GetMap());

        await Clients
            .Group(roomCode)
            .SendAsync("UserJoined", username);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task PlaceTower(string roomCode, int x, int y)
    {
        if (_gameService.Rooms.TryGetValue(roomCode, out var gameState))
        {
             gameState.QueueTowerPlacement(x, y);
        }
    }
}
