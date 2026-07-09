// ReSharper disable once CheckNamespace
namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents domain errors that occur during application execution
/// </summary>
#pragma warning disable S3925
public class DomainException : Exception
#pragma warning restore S3925
{
    /// <inheritdoc />
    public DomainException()
    {
    }

    /// <inheritdoc />
    public DomainException(string message) : base(message)
    {
    }


    /// <inheritdoc />
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Domain context for this exception
    /// </summary>
    public virtual string Context => nameof(DomainException);
}
