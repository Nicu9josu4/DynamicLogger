using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DynamicLogger.Services
{
    public class DynamicLoggerChanger(ILogger innerLogger, string category) : ILogger
    {
        private static readonly ConcurrentDictionary<string, LogLevel> _dynamicLevels = new();
        private static readonly string DefaultCategory = "Default";

        public string CategoryName { get; } = category;

        public static Dictionary<string, string> GetCategories() => _dynamicLevels.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

        public static void SetLogLevelToNonSystemCategories(LogLevel level)
        {
            foreach (var levelCategory in _dynamicLevels.Where(levelCategory => !levelCategory.Key.StartsWith("Microsoft.") && !levelCategory.Key.StartsWith("System.")))
                _dynamicLevels[levelCategory.Key] = level;
        }

        public static void SetLogLevelToPrefixCategory(string prefix, LogLevel level)
        {
            foreach (var levelCategory in _dynamicLevels.Where(levelCategory => levelCategory.Key.StartsWith(prefix)))
                _dynamicLevels[levelCategory.Key] = level;
        }

        public static void SetLogLevel(string category, LogLevel level)
        {
            if (category == DefaultCategory)
            {
                foreach (var levelCategory in _dynamicLevels)
                {
                    _dynamicLevels[levelCategory.Key] = level;
                }
            }
            _dynamicLevels[category] = level;
        }

        public static LogLevel GetLogLevel(string category)
        {
            // If specific category exists, return its log level
            if (_dynamicLevels.TryGetValue(category, out var logLevel))
                return logLevel;

            // If not, add with default log level and return
            if (_dynamicLevels.TryGetValue(DefaultCategory, out var defaultLevel))
            {
                _dynamicLevels.TryAdd(category, defaultLevel);
                return defaultLevel;
            }

            // If DefaultCategory is missing, register with Information (or whatever you want default)
            _dynamicLevels.TryAdd(category, LogLevel.Information);
            return LogLevel.Information;
        }

        public IDisposable BeginScope<TState>(TState state) => innerLogger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel)
        {
            var currentLevel = GetLogLevel(CategoryName);
            return logLevel >= currentLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            innerLogger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
