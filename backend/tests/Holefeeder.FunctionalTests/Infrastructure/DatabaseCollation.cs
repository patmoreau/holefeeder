using System.Globalization;

namespace Holefeeder.FunctionalTests.Infrastructure;

/// <summary>
///     Orders strings the way the test database does. PostgreSQL sorts with the libc
///     en_US.utf8 collation, which ignores spaces and punctuation on its first pass,
///     while <see cref="Comparer{T}" />.Default weighs them like any other character.
///     Assertions on a database-ordered collection must use this comparer or they fail
///     on values such as "Vel rerum" and "Velit".
/// </summary>
internal static class DatabaseCollation
{
    public static StringComparer Comparer { get; } =
        StringComparer.Create(CultureInfo.InvariantCulture, CompareOptions.IgnoreSymbols);
}
