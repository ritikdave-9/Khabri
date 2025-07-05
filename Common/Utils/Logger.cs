using Serilog;
using System;

namespace Common.Utils
{
    public static class Logger
    {
        private static readonly ILogger _logger;

        static Logger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static void LogInformation(string message)
        {
            _logger.Information($"{GetTimestamp()} {message}");
        }

        public static void LogError(string message)
        {
            _logger.Error($"{GetTimestamp()} {message}");
        }

        public static void LogSuccess(string message)
        {
            _logger.Information($"{GetTimestamp()} [SUCCESS] {message}");
        }

        private static string GetTimestamp()
        {
            return $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC]";
        }
    }
}