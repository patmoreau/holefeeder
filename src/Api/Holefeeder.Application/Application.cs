using Microsoft.Extensions.Logging;

namespace Holefeeder.Application;

public static class EventIds
{
    public static EventId ApplicationStarted { get; } = new(1000, nameof(ApplicationStarted));
    public static EventId ApplicationStopped { get; } = new(1001, nameof(ApplicationStopped));
}

public sealed class Application;
