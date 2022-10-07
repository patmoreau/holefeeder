using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.SeedWork;

public class DomainException : Exception
{
    protected DomainException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }

    public virtual string Context => nameof(DomainException);

    protected DomainException()
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }

    public DomainException(string message) : base(message)
    {
    }


    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
