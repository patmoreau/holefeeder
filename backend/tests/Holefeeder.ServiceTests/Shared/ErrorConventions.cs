using System.Reflection;
using System.Text.RegularExpressions;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.ServiceTests.Shared;

public partial class ErrorConventions
{
    private static readonly Type[] ErrorClasses = typeof(Holefeeder.Service.Api).Assembly.GetTypes()
        .Where(type => type is { IsAbstract: true, IsSealed: true } && type.Name.EndsWith("Errors", StringComparison.Ordinal))
        .Where(type => RingNamespace().IsMatch(type.Namespace ?? string.Empty))
        .OrderBy(type => type.FullName, StringComparer.Ordinal)
        .ToArray();

    public static TheoryData<string> ErrorClassNames => [.. ErrorClasses.Select(type => type.FullName!)];

    [Fact]
    public void ErrorClassesAreDiscovered() =>
        ErrorClasses.Select(type => type.Name).Should().Contain(["CodeErrors", "OffsetErrors", "UsersErrors"]);

    [Theory]
    [MemberData(nameof(ErrorClassNames))]
    public void ErrorCodesAreKebabCase(string errorClassName) =>
        CodesOf(ClassNamed(errorClassName)).Should().AllSatisfy(code => code.Should().MatchRegex(KebabCase().ToString()));

    [Theory]
    [MemberData(nameof(ErrorClassNames))]
    public void ErrorMessagesAreSentences(string errorClassName) =>
        ErrorsOf(ClassNamed(errorClassName)).Should().AllSatisfy(error => error.Description.Should().MatchRegex(Sentence().ToString()));

    [Fact]
    public void ErrorCodesAreUnique() =>
        ErrorClasses.SelectMany(CodesOf).Should().OnlyHaveUniqueItems();

    private static Type ClassNamed(string fullName) => ErrorClasses.Single(type => type.FullName == fullName);

    private static IEnumerable<string> CodesOf(Type errorClass) =>
        errorClass.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field is { IsLiteral: true } && field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!);

    private static IEnumerable<ResultError> ErrorsOf(Type errorClass) =>
        errorClass.GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(ResultError))
            .Select(property => (ResultError)property.GetValue(null)!);

    [GeneratedRegex(@"^Holefeeder\.Service\.[^.]+\.(Api|Core|Events|Jobs|Persistence|Ui)(\.|$)")]
    private static partial Regex RingNamespace();

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex KebabCase();

    [GeneratedRegex(@"^[A-Z].*\.$")]
    private static partial Regex Sentence();
}
