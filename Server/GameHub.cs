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

        var player = new Player(username, Context.ConnectionId);
        gameState.AddPlayer(player);

        await Clients.Caller.SendAsync("InitializeMap", gameState.Map.Width, gameState.Map.Height, gameState.GetMap());

        var activeUsernames = gameState.GetPlayersUsernames();

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

                var activeUsernames = gameState.GetPlayersUsernames();
                await Clients.Group(room.Key).SendAsync("UserLeft", player.Username, activeUsernames);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task PlaceTower(string roomCode, int x, int y)
    {
        if (_gameService.Rooms.TryGetValue(roomCode, out var gameState))
        {
             gameState.QueueTowerPlacement(x, y);
        }
    }
}
