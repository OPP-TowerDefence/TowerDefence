
namespace TowerDefense.Utils
{
    public class SerilogAdapter : Interfaces.ILogger
    {
        private readonly Serilog.ILogger _serilog;

        public SerilogAdapter(Serilog.ILogger serilog)
        {
            _serilog = serilog;
        }
        public void LogError(string message)
        {
            _serilog.Error(message);
        }

        public void LogException(Exception exception)
        {
            _serilog.Error(exception, exception.Message);
        }

        public void LogInfo(string message)
        {
            _serilog.Information(message);
        }
    }
}
