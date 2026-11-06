using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.ServiceTests.Shared.Core.Search;

public class SearchQueryParserTests
{
    private static readonly ResultError NotANumber = new("not-a-number", "Value is not a number.");
    private static readonly ResultError Malformed = new("malformed", "Value is malformed.");
    private static readonly ResultError Rejected = new("rejected", "Value is rejected.");
    private static readonly (string, SortByDirection) DefaultSort = ("default", SortByDirection.Ascending);
    private static readonly (string, FilterByOperator, string) DefaultFilter = ("default", FilterByOperator.Equal, "default");

    [Fact]
    public void ParseOrDefault_ReturnsDefault_WhenRawIsNull()
    {
        var result = SearchQueryParser.ParseOrDefault(null, Limit.Default, Limit.Create, NotANumber);

        result.Should().BeSuccessful().And.WithValue(Limit.Default);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    [InlineData(" 42 ", 42)]
    [InlineData("+7", 7)]
    public void ParseOrDefault_ReturnsCreatedValue_WhenRawIsValidNumber(string raw, int expected)
    {
        var result = SearchQueryParser.ParseOrDefault(raw, Limit.Default, Limit.Create, NotANumber);

        result.Should().BeSuccessful().And.WithValue(Limit.Valid(expected));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("101")]
    [InlineData("-1")]
    public void ParseOrDefault_ReturnsCreateError_WhenNumberIsRejectedByCreate(string raw)
    {
        var result = SearchQueryParser.ParseOrDefault(raw, Limit.Default, Limit.Create, NotANumber);

        result.Should().BeFailure().And.WithError(LimitErrors.Invalid);
    }

    [Fact]
    public void ParseOrDefault_UsesGivenCreate_WhenParsingOffset()
    {
        var result = SearchQueryParser.ParseOrDefault("-1", Offset.Default, Offset.Create, NotANumber);

        result.Should().BeFailure().And.WithError(OffsetErrors.Invalid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abc")]
    [InlineData("1.5")]
    [InlineData("1,000")]
    [InlineData("0x10")]
    [InlineData("99999999999")]
    public void ParseOrDefault_ReturnsNotANumberError_WhenRawIsNotAnInteger(string raw)
    {
        var result = SearchQueryParser.ParseOrDefault(raw, Limit.Default, Limit.Create, NotANumber);

        result.Should().BeFailure().And.WithError(NotANumber);
    }

    [Fact]
    public void ParseSortBy_ReturnsDefault_WhenSortIsNull()
    {
        var result = ParseSort(null);

        result.Should().BeSuccessful().And.WithValue(DefaultSort);
    }

    [Theory]
    [InlineData("name", "name")]
    [InlineData("Name_2", "Name_2")]
    public void ParseSortBy_ReturnsAscending_WhenFieldHasNoPrefix(string sort, string expectedField)
    {
        var result = ParseSort(sort);

        result.Should().BeSuccessful().And.WithValue((expectedField, SortByDirection.Ascending));
    }

    [Fact]
    public void ParseSortBy_ReturnsDescending_WhenFieldIsPrefixedWithDash()
    {
        var result = ParseSort("-name");

        result.Should().BeSuccessful().And.WithValue(("name", SortByDirection.Descending));
    }

    [Fact]
    public void ParseSortBy_ReturnsCreateError_WhenCreateFails()
    {
        var result = SearchQueryParser.ParseSortBy(
            "name", DefaultSort, (_, _) => Rejected, Malformed);

        result.Should().BeFailure().And.WithError(Rejected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData("--name")]
    [InlineData("+name")]
    [InlineData("first-name")]
    [InlineData("first name")]
    [InlineData(" name")]
    [InlineData("name:asc")]
    public void ParseSortBy_ReturnsGivenError_WhenSortIsMalformed(string sort)
    {
        var result = ParseSort(sort);

        result.Should().BeFailure().And.WithError(Malformed);
    }

    [Fact]
    public void ParseFilterBy_ReturnsDefault_WhenFilterIsNull()
    {
        var result = ParseFilter(null);

        result.Should().BeSuccessful().And.WithValue(DefaultFilter);
    }

    [Theory]
    [InlineData("eq", nameof(FilterByOperator.Equal))]
    [InlineData("ne", nameof(FilterByOperator.NotEqual))]
    [InlineData("lt", nameof(FilterByOperator.LessThan))]
    [InlineData("le", nameof(FilterByOperator.LessThanOrEqual))]
    [InlineData("gt", nameof(FilterByOperator.GreaterThan))]
    [InlineData("ge", nameof(FilterByOperator.GreaterThanOrEqual))]
    public void ParseFilterBy_ReturnsMatchingOperator_WhenOperatorIsKnown(string op, string expectedOperator)
    {
        var result = ParseFilter($"code:{op}:settings");

        result.Should().BeSuccessful().And.WithValue(("code", Enum.Parse<FilterByOperator>(expectedOperator), "settings"));
    }

    [Fact]
    public void ParseFilterBy_ReturnsCreateError_WhenCreateFails()
    {
        var result = SearchQueryParser.ParseFilterBy(
            "code:eq:settings", DefaultFilter, (_, _, _) => Rejected, Malformed);

        result.Should().BeFailure().And.WithError(Rejected);
    }

    [Theory]
    [InlineData("code:eq:a:b", "a:b")]
    [InlineData("code:eq:hello world", "hello world")]
    [InlineData("code:eq:-1", "-1")]
    [InlineData("code:eq: ", " ")]
    public void ParseFilterBy_KeepsWholeValue_WhenValueContainsSpecialCharacters(string filter, string expectedValue)
    {
        var result = ParseFilter(filter);

        result.Should().BeSuccessful().And.WithValue(("code", FilterByOperator.Equal, expectedValue));
    }

    [Theory]
    [InlineData("")]
    [InlineData("code")]
    [InlineData("code:eq")]
    [InlineData("code:eq:")]
    [InlineData(":eq:settings")]
    [InlineData("code::settings")]
    [InlineData("first-name:eq:settings")]
    [InlineData("code:e-q:settings")]
    public void ParseFilterBy_ReturnsGivenError_WhenFilterIsMalformed(string filter)
    {
        var result = ParseFilter(filter);

        result.Should().BeFailure().And.WithError(Malformed);
    }

    [Theory]
    [InlineData("code:xx:settings")]
    [InlineData("code:EQ:settings")]
    [InlineData("code:equal:settings")]
    public void ParseFilterBy_ReturnsGivenError_WhenOperatorIsUnknown(string filter)
    {
        var result = ParseFilter(filter);

        result.Should().BeFailure().And.WithError(Malformed);
    }

    private static Result<(string Field, SortByDirection Direction)> ParseSort(string? sort) =>
        SearchQueryParser.ParseSortBy<(string, SortByDirection)>(
            sort, DefaultSort, (field, direction) => (field, direction), Malformed);

    private static Result<(string Field, FilterByOperator Operator, string Value)> ParseFilter(string? filter) =>
        SearchQueryParser.ParseFilterBy<(string, FilterByOperator, string)>(
            filter, DefaultFilter, (field, filterByOperator, value) => (field, filterByOperator, value), Malformed);
}
