using Microsoft.Extensions.Logging;

namespace DynamicLogger.Services
{
    public class DynamicLoggerProvider(ILoggerProvider innerProvider) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new DynamicLoggerChanger(innerProvider.CreateLogger(categoryName), categoryName);
        }

        public void Dispose() => innerProvider.Dispose();
    }
}
