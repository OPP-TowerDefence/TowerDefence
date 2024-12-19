using TowerDefense.Models.Mementos;

namespace TowerDefense.Utils
{
    public class GameCaretaker
    {
        private readonly Stack<GameStateMemento> _mementoStack = new();

        public void Save(GameStateMemento memento)
        {
            _mementoStack.Push(memento);
            Logger.Instance.LogInfo("Game state saved to caretaker.");
        }

        public GameStateMemento? Restore()
        {
            if (_mementoStack.Count > 0)
            {
                Logger.Instance.LogInfo("Restoring previous game state from caretaker.");
                return _mementoStack.Pop();
            }

            Logger.Instance.LogError("No previous game state to restore.");
            return null;
        }
    }
}
