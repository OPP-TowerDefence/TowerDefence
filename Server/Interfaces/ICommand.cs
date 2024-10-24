namespace TowerDefense.Interfaces
{
    public interface ICommand
    {
        void Execute();

        void Undo();
    }
}
