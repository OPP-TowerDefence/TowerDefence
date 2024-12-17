using Microsoft.AspNetCore.SignalR;
using TowerDefense.Enums;
using TowerDefense.Models;
using TowerDefense.Services;
using TowerDefense.Utils;

namespace TowerDefense
{
    public class GameHub(GameService gameService, Interfaces.ILogger logger) : Hub
    {
        private static readonly Dictionary<string, GameCaretaker> _caretakers = new();
        private readonly GameService _gameService = gameService;
        private readonly Interfaces.ILogger _logger = logger;

        public async Task JoinRoom(string roomCode, string username)
        {
            _logger.LogInfo($"Player {username} is attempting to join room {roomCode}.");

            if (!_gameService.Rooms.TryGetValue(roomCode, out var gameState))
            {
                gameState = new GameState(Context.GetHttpContext()!.RequestServices.GetService<IHubContext<GameHub>>()!, roomCode);

                _gameService.Rooms.TryAdd(roomCode, gameState);

                _logger.LogInfo($"New room created with code: {roomCode}.");
            }
            else if (gameState.GameStarted)
            {
                await Clients.Caller.SendAsync("Game has already started.");

                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            gameState.AddPlayer(username, Context.ConnectionId);

            await Clients.Caller.SendAsync("InitializeMap", gameState.Map.Width, gameState.Map.Height, gameState.GetMapTowers(), gameState.GetMapEnemies(), gameState.SendPath(), gameState.GetMainObject(), gameState.GetMapBullets());

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
                var player = room.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

                if (player != null)
                {
                    room.RemovePlayer(Context.ConnectionId);

                    _logger.LogInfo($"Player {player.Username} left room {room.RoomCode}.");

                    var activeUsernames = room.GetActivePlayers();

                    if (room.Players.Count == 0)
                    {
                        _gameService.Rooms.Remove(room.RoomCode, out _);

                        _logger.LogInfo($"Room {room.RoomCode} has been removed.");
                    }
                    else
                    {
                        await Clients
                            .Group(room.RoomCode)
                            .SendAsync("UserLeft", player.Username, activeUsernames);
                    }
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
            if (_gameService.Rooms.TryGetValue(roomCode, out var room) && room is not null)
            {
                var player = room.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

                if (player is not null)
                {
                    room.PlaceTower(x, y, towerCategory, player);
                }
                else
                {
                    _logger.LogError("Failed to place tower: Player not found.");
                }
            }
            else
            {
                _logger.LogError($"Failed to place tower: room code {roomCode} does not exist.");
            }
        }

        public async Task RestoreGame(string roomCode)
        {
            if (_gameService.Rooms.TryGetValue(roomCode, out var room) && room is not null)
            {
                if (_caretakers.ContainsKey(roomCode))
                {
                    var memento = _caretakers[roomCode].Restore();
                    if (memento != null)
                    {
                        room.RestoreState(memento);
                        await Clients.Caller.SendAsync("DisplayMessage", "Game state restored successfully!");
                    }
                }
            }
            else
            {
                _logger.LogError($"Room {roomCode} not found for restoring game state.");
            }
        }

        public async Task SaveGame(string roomCode)
        {
            if (_gameService.Rooms.TryGetValue(roomCode, out var room) && room is not null)
            {
                if (!_caretakers.ContainsKey(roomCode))
                {
                    _caretakers[roomCode] = new GameCaretaker();
                }

                _caretakers[roomCode].Save(room.SaveState());
                await Clients.Caller.SendAsync("DisplayMessage", "Game state saved successfully!");
            }
            else
            {
                _logger.LogError($"Room {roomCode} not found for saving game state.");
            }
        }

        public async Task StartGame(string roomCode, string username)
        {
            if (_gameService.Rooms.TryGetValue(roomCode, out var room) && room is not null && !room.GameStarted)
            {
                room.StartGame(Context.ConnectionId);

                await Clients
                    .Group(roomCode)
                    .SendAsync("GameStarted", $"Game has been started by {username}!");
            }
            else
            {
                _logger.LogError($"Could not start game: room code {roomCode} does not exist or the game has already started.");
            }
        }

        public async Task UndoTower(string roomCode)
        {
            if (_gameService.Rooms.TryGetValue(roomCode, out var room) && room is not null)
            {
                var player = room.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

                if (player is not null)
                {
                    room.UndoLastCommand(player.ConnectionId);
                }
                else
                {
                    _logger.LogError("Failed to undo tower: Player not found.");
                }
            }
            else
            {
                _logger.LogError($"Failed to undo tower: room code {roomCode} does not exist.");
            }
        }

        public async Task UpgradeTower(string roomCode, int x, int y, TowerUpgrades towerUpgrade)
        {
            if (_gameService.Rooms.TryGetValue(roomCode, out var room) && room is not null)
            {
                room.UpgradeTower(x, y, towerUpgrade);
            }
            else
            {
                _logger.LogError($"Failed to upgrade tower: room code {roomCode} does not exist.");
            }
        }
    }
}
