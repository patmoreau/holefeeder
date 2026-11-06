using Microsoft.Extensions.Logging;

namespace Holefeeder.ServiceTests.TestKit;

internal sealed class LoggerForTest<T>(ITestOutputHelper testOutputHelper) : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        testOutputHelper.WriteLine($"[{logLevel}] {formatter(state, exception)}");
        if (exception is not null)
        {
            testOutputHelper.WriteLine(exception.ToString());
        }
    }
}
