using System;
using System.IO;
using TurnBasedGame.Classes;

namespace TurnBasedGame.Classes.Logging
{
    /// <summary>
    /// Logger that writes to a text file
    /// </summary>
    public class FileLogger : ILogListener, IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly string _logFilePath;
        private readonly object _lock = new object();

        /// <summary>
        /// Writes a log message to the file.
        /// </summary>
        /// <param name="logDirectory">The directory of saved logs.</param>
        /// <param name="baseFileName">The name of a file for game logs.</param>
        public FileLogger(string logDirectory = "Logs", string baseFileName = "game")
        {
            // Ensure log directory exists
            Directory.CreateDirectory(logDirectory);

            // Create timestamped log file
            _logFilePath = Path.Combine(logDirectory, $"{baseFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            _writer = new StreamWriter(_logFilePath, append: true)
            {
                AutoFlush = true
            };

            WriteLog(LogLevel.Info, "=== Logging started ===");
        }

        /// <summary>
        /// Write log, with time, loglevel and message
        /// </summary>
        /// <param name="level">The severity level of the log message.</param>
        /// <param name="message">The log message to output.</param>
        public void WriteLog(LogLevel level, string message)
        {
            lock (_lock)
            {
                try
                {
                    _writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}");
                }
                catch (Exception ex)
                {
                    // Message to console if file writing fails
                    Console.WriteLine($"Failed to write log: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Releases resources used by the logger and finalizes the log file.
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                try
                {
                    WriteLog(LogLevel.Info, "=== Logging stopped ===");
                    _writer?.Dispose();
                }
                catch
                {
                    System.Console.WriteLine( Console.Error );
                }
            }
        }
    }
}