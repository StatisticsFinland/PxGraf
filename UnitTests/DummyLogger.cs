using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    /// <summary>
    /// Minimal implementation of the ILogger interface for testing purposes.
    /// </summary>
    class DummyLogger : ILogger
    {
        private readonly List<LogLevel> _enabledLevels;
        private readonly LoggerExternalScopeProvider _scopeProvider;
        private readonly ILogWriter _writer;

        /// <summary>
        /// Constructs a minimal logger object that uses the provided writer object for writing the log.
        /// </summary>
        /// <param name="writer">Provides the writing functionality for the logger</param>
        public DummyLogger(ILogWriter writer)
        {
           _enabledLevels = new List<LogLevel>
           {
               LogLevel.Trace,
               LogLevel.Information,
               LogLevel.Warning,
               LogLevel.Error,
               LogLevel.Critical,
               LogLevel.Debug,
               LogLevel.None
           };

            _scopeProvider = new LoggerExternalScopeProvider();
            _writer = writer;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _scopeProvider.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _enabledLevels.Contains(logLevel);
        }

        public bool DisableLogLevel(LogLevel logLevel)
        {
            if (_enabledLevels.Contains(logLevel))
            {
                _enabledLevels.Remove(logLevel);
                return true;
            }
            return false;
        }

        public bool EnableLogLevel(LogLevel logLevel)
        {
            if (_enabledLevels.Contains(logLevel))
            {
                return false;
            }
            _enabledLevels.Add(logLevel);
            return true;
        }

        /// <summary>
        /// Calls the writer provided in the constructor
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _writer.Write(new LogEntry(logLevel, eventId, state, exception).ToString());
        }

        /// <summary>
        /// Minimal interface for writing to a file
        /// </summary>
        public interface ILogWriter
        {
            public void Write(string toBeWritten);
        }

        /// <summary>
        /// Class for handling the parameters used in ILogger.Log method
        /// </summary>
        private class LogEntry
        {
            private readonly LogLevel _level;
            private readonly EventId _eventId;
            private readonly object _state;
            private readonly Exception _exception;

            public LogEntry(LogLevel logLevel, EventId eventId, object state, Exception exception)
            {
                _level = logLevel;
                _eventId = eventId;
                _state = state;
                _exception = exception;
            }

            public override string ToString()
            {
                StringBuilder messageBuilder = new(_level.ToString() + " " + _eventId);
                if (_state != null) messageBuilder.Append(" " + _state.ToString());
                if (_exception != null) messageBuilder.Append(" " + _exception.Message);

                return messageBuilder.ToString();
            }
        }
    }
}
