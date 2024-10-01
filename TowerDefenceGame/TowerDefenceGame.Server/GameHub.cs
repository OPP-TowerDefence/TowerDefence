using Microsoft.AspNetCore.SignalR;
using TowerDefenceGame.Server.Entities;

namespace TowerDefenceGame.Server
{
    public class GameHub : Hub
    {
        private static Map _mapGrid = new Map(10, 10);

        public async Task PlaceTower(int x, int y)
        {
            if (_mapGrid.PlaceTower(x, y))
            {
                await Clients.All.SendAsync("ReceiveTowerPlacement", x, y);
            }
        }
    }
}
