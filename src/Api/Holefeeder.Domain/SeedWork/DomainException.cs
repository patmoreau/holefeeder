namespace Holefeeder.Domain.SeedWork;

public class DomainException : Exception
{
    protected DomainException()
    {
    }

    protected DomainException(string message) : base(message)
    {
    }


    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public virtual string Context => nameof(DomainException);
}
