namespace Holefeeder.Domain.SeedWork;

public class DomainException<TContext> : DomainException
{
    protected DomainException() => Context = typeof(TContext).Name;

    protected DomainException(string message) : base(message) => Context = typeof(TContext).Name;

    protected DomainException(string message, Exception innerException) : base(message, innerException) =>
        Context = nameof(TContext);

    public override string Context { get; }
}
