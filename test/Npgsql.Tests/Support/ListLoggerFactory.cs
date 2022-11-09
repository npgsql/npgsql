using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Npgsql.Tests.Support;

public class ListLoggerProvider : ILoggerProvider
{
    readonly ListLogger _logger;
    bool _recording;

    public ListLoggerProvider()
        => _logger = new ListLogger(this);

    public List<(LogLevel Level, EventId Id, string Message, object? State, Exception? Exception)> Log
        => _logger.LoggedEvents;

    public IDisposable Record()
    {
        _logger.Clear();
        _recording = true;

        return new RecordingDisposable(this);
    }

    public void StopRecording()
        => _recording = false;

    public ILogger CreateLogger(string categoryName) => _logger;

    public void AddProvider(ILoggerProvider provider)
    {
    }

    public void Dispose()
        => StopRecording();

    class ListLogger : ILogger
    {
        readonly ListLoggerProvider _provider;

        public ListLogger(ListLoggerProvider provider)
            => _provider = provider;

        public List<(LogLevel, EventId, string, object?, Exception?)> LoggedEvents { get; }
            = new();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (_provider._recording)
            {
                lock (this)
                {
                    var message = formatter(state, exception).Trim();
                    LoggedEvents.Add((logLevel, eventId, message, state, exception));
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                LoggedEvents.Clear();
            }
        }

        public bool IsEnabled(LogLevel logLevel) => _provider._recording;

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
            => new Scope();

        class Scope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    class RecordingDisposable : IDisposable
    {
        readonly ListLoggerProvider _provider;

        public RecordingDisposable(ListLoggerProvider provider)
            => _provider = provider;

        public void Dispose()
            => _provider.StopRecording();
    }
}
