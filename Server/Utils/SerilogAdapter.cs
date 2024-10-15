
namespace TowerDefense.Utils
{
    public class SerilogAdapter : Interfaces.ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogAdapter(Serilog.ILogger logger)
        {
            _logger = logger;
        }
        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogException(Exception exception)
        {
            _logger.Error(exception, exception.Message);
        }

        public void LogInfo(string message)
        {
            _logger.Information(message);
        }
    }
}
