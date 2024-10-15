namespace TowerDefense.Interfaces
{
    public interface ILogger
    {
        public void LogError(string message);
        public void LogException(Exception exception);
        public void LogInfo(string message);
    }
}
