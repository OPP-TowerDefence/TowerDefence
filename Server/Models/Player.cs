namespace TowerDefense.Models
{
    public class Player
    {
        public string Username { get; set; }
        public string ConnectionId { get; set; }

        public Player(string username, string connectionId)
        {
            Username = username;
            ConnectionId = connectionId;
        }
    }
}
