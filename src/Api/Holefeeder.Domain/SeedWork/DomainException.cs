using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.SeedWork;

public abstract class DomainException : Exception
{
    protected DomainException(int? statusCode, string message) : base(message)
    {
        StatusCode = statusCode ?? StatusCodes.Status500InternalServerError;
    }

    public int StatusCode { get; }

    public abstract string Context { get; }
}
