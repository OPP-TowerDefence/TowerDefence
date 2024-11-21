using System.Collections;
using System.Collections.Concurrent;

namespace TowerDefense.Models.Collections
{
    public class RoomCollection : IEnumerable<GameState>
    {
        private readonly ConcurrentDictionary<string, GameState> _rooms = new();

        public IEnumerator<GameState> GetEnumerator()
        {
            foreach (var room in _rooms)
            {
                yield return room.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(string roomCode, out GameState? room) => _rooms.Remove(roomCode, out room);

        public bool TryAdd(string roomCode, GameState room) => _rooms.TryAdd(roomCode, room);

        public bool TryGetValue(string roomCode, out GameState? room) => _rooms.TryGetValue(roomCode, out room);
    }
}