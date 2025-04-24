using System;
using System.Collections.Generic;
using System.IO;
using TurnBasedGame.Classes.Logging;

namespace TurnBasedGame.Classes.Logging
{
    /// <summary>
    /// Represents the severity level of a log message.
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Interface for logging functionality (Strategy pattern).
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="level">The severity level of the message.</param>
        /// <param name="message">The message to log.</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Adds a listener that will receive log messages.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        void AddListener(ILogListener listener);

        /// <summary>
        /// Removes a listener from receiving log messages.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        void RemoveListener(ILogListener listener);
    }

    /// <summary>
    /// Interface for objects that listen to log events (Observer pattern).
    /// </summary>
    public interface ILogListener
    {
        /// <summary>
        /// Receives a log message.
        /// </summary>
        /// <param name="level">The severity level of the message.</param>
        /// <param name="message">The log message content.</param>
        void WriteLog(LogLevel level, string message);
    }
}

/// <summary>
/// Concrete logger that distributes log messages to registered listeners.
/// </summary>
public class GameLogger : ILogger
{
    private readonly List<ILogListener> _listeners = new List<ILogListener>();

    /// <summary>
    /// Minimum level of log messages to output.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    /// <inheritdoc />
    public void Log(LogLevel level, string message)
    {
        if (level < MinimumLevel) return;

        foreach (var listener in _listeners)
        {
            listener.WriteLog(level, message);
        }
    }

    /// <inheritdoc />
    public void AddListener(ILogListener listener)
    {
        _listeners.Add(listener);
        Log(LogLevel.Debug, $"Added listener: {listener.GetType().Name}");
    }

    /// <inheritdoc />
    public void RemoveListener(ILogListener listener)
    {
        _listeners.Remove(listener);
        Log(LogLevel.Debug, $"Removed listener: {listener.GetType().Name}");
    }
}
