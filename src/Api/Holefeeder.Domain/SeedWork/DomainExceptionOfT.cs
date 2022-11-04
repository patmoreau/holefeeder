namespace Holefeeder.Domain.SeedWork;

#pragma warning disable CA1032
public class DomainException<TContext> : DomainException
#pragma warning restore CA1032
{
    public DomainException(string message) : base(message)
    {
        Context = typeof(TContext).Name;
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
        Context = nameof(TContext);
    }

    public override string Context { get; }
}
