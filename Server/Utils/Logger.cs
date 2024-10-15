namespace TowerDefense.Utils
{
    public sealed class Logger : Interfaces.ILogger
    {
        private static readonly Logger _instance = new();
        private readonly string _logFilePath;

        public static Logger Instance
        {
            get
            {
                return _instance;
            }
        }

        private Logger()
        {
            _logFilePath = "log.txt";

            if (!File.Exists(_logFilePath))
            {
                File
                    .Create(_logFilePath)
                    .Dispose();
            }
        }

        private void Log(string message)
        {
            string logEntry = $"{DateTime.Now}: {message}";

            Console.WriteLine(logEntry);

            try
            {
                lock (_logFilePath)
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        public void LogError(string message)
        {
            Log($"ERROR: {message}");
        }

        public void LogException(Exception ex)
        {
            Log($"EXCEPTION: {ex.GetType().Name} - {ex.Message}");
        }

        public void LogInfo(string message)
        {
            Log($"INFO: {message}");
        }
    }
}
