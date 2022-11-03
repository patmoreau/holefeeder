using DrifterApps.Holefeeder.Core.Domain;

namespace Holefeeder.Application.Exceptions;

#pragma warning disable CA1032
public abstract class NotFoundException : Exception
#pragma warning restore CA1032
{
    protected NotFoundException(Guid id, string context) : base($"'{id}' not found")
    {
        Context = context;
    }

    public string Context { get; }
}

#pragma warning disable CA1032
public class NotFoundException<TContext> : NotFoundException where TContext : IEntity
#pragma warning restore CA1032
{
    public NotFoundException(Guid id) : base(id, nameof(TContext))
    {
    }
}
