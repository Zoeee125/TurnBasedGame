using System;

namespace TurnBasedGame.Classes.Logging
{
    /// <summary>
    /// Logger that outputs to the console with colored text
    /// </summary>
    public class ConsoleLogger : ILogListener
    {
        /// <summary>
        /// Writes a log message to the console with color based on log level.
        /// </summary>
        /// 
        /// <param name="level">The severity level of the log message.</param>
        /// <param name="message">The log message to output.</param>
        public void WriteLog(LogLevel level, string message)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = GetColorForLevel(level);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}");
            Console.ForegroundColor = originalColor;
        }

        private static ConsoleColor GetColorForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => ConsoleColor.DarkGray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };
        }

    }
}