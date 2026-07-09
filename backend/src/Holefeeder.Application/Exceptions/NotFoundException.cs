using DrifterApps.Seeds.Domain;

namespace Holefeeder.Application.Exceptions;

public abstract class NotFoundException : Exception
{
    protected NotFoundException(Guid id, string context) : base($"'{id}' not found") => Context = context;

    protected NotFoundException() => Context = string.Empty;

    protected NotFoundException(string message, Exception innerException) : base(message, innerException) =>
        Context = string.Empty;

    protected NotFoundException(string message) : base(message) => Context = string.Empty;

    public string Context { get; }
}

public class NotFoundException<TContext> : NotFoundException where TContext : IAggregateRoot
{
    protected NotFoundException(Guid id) : base(id, nameof(TContext))
    {
    }

    protected NotFoundException()
    {
    }

    protected NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected NotFoundException(string message) : base(message)
    {
    }
}
