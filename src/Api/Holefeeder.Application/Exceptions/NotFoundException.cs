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
    public NotFoundException(Guid id) : base(id, nameof(TContext))
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
