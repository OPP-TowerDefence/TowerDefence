﻿using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Timers;
using TowerDefense.Models;

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
    }

    public ConcurrentDictionary<string, GameState> Rooms => _rooms;

    private async void GameTickHandler(object sender, ElapsedEventArgs e)
    {
        _timeSinceLastSpawn += 250;

        foreach (var room in _rooms)
        {
            var gameState = room.Value;

            gameState.ProcessTowerPlacements();

            if (_timeSinceLastSpawn >= SpawnInterval)
            {
                gameState.SpawnEnemies();
                _timeSinceLastSpawn = 0;
            }

            gameState.UpdateEnemies();

            await _hubContext.Clients
                .Group(room.Key)
                .SendAsync("OnTick", gameState.GetMapTowers(), gameState.GetMapEnemies());
        }
    }
}