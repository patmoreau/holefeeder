// ReSharper disable once CheckNamespace
namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents domain errors that occur during application execution
/// </summary>
/// <typeparam name="TContext">Context type for exception</typeparam>
#pragma warning disable S3925
public class DomainException<TContext> : DomainException
#pragma warning restore S3925
{
    private static readonly string ContextTypeName = typeof(TContext).Name;

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

    /// <inheritdoc />
    public override string Context => ContextTypeName;
}
