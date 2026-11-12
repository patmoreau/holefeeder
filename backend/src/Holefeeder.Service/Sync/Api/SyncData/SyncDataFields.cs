using System.Globalization;
using System.Text.Json;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Api.SyncData;

internal sealed class SyncDataFields(IReadOnlyDictionary<string, JsonElement> data, ResultAggregate validation)
{
    private const string DateFormat = "yyyy-MM-dd";

    public string? Text(string key) =>
        Value(key) switch
        {
            null => null,
            { ValueKind: JsonValueKind.String } json => json.GetString(),
            { } json => json.GetRawText()
        };

    public bool? Flag(string key) =>
        Value(key) switch
        {
            null => null,
            { ValueKind: JsonValueKind.True } => true,
            { ValueKind: JsonValueKind.False } => false,
            { ValueKind: JsonValueKind.Number } json when json.TryGetInt64(out var number) => number != 0,
            _ => Invalid<bool>(key, "0 or 1")
        };

    public int? Number(string key) =>
        Value(key) switch
        {
            null => null,
            { ValueKind: JsonValueKind.Number } json when json.TryGetInt32(out var number) => number,
            _ => Invalid<int>(key, "a whole number")
        };

    public Money? Cents(string key) =>
        Value(key) switch
        {
            null => null,
            { ValueKind: JsonValueKind.Number } json when json.TryGetInt64(out var cents) && cents >= 0 => Money.Valid(cents / 100m),
            _ => InvalidReference<Money>(key, "zero or more cents as a whole number")
        };

    public DateOnly? Date(string key) =>
        Value(key) switch
        {
            null => null,
            { ValueKind: JsonValueKind.String } json when DateOnly.TryParseExact(
                json.GetString(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) => date,
            _ => Invalid<DateOnly>(key, $"a date as {DateFormat}")
        };

    public Id? Reference(string key) =>
        Value(key) switch
        {
            null => null,
            { ValueKind: JsonValueKind.String } json when Guid.TryParse(json.GetString(), out var guid) => Id.Valid(guid),
            _ => InvalidReference<Id>(key, "an id")
        };

    public TagList? Tags(string key)
    {
        if (!data.TryGetValue(key, out var json))
        {
            return null;
        }

        return json.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => TagList.Create([]),
            JsonValueKind.String => TagList.Create(json.GetString()?.Split(',')),
            JsonValueKind.Array when json.EnumerateArray().All(tag => tag.ValueKind == JsonValueKind.String) =>
                TagList.Create(json.EnumerateArray().Select(tag => tag.GetString())),
            _ => InvalidReference<TagList>(key, "a comma separated text or a list of texts")
        };
    }

    public AccountType? AccountType(string key) => Parsed(key, Shared.Core.AccountType.Create);

    public IntervalType? IntervalType(string key) => Parsed(key, Shared.Core.IntervalType.Create);

    public CategoryType? CategoryType(string key)
    {
        var text = Text(key);
        if (text is null)
        {
            return null;
        }

        var name = Enum.GetNames<CategoryType>().FirstOrDefault(name => string.Equals(name, text, StringComparison.OrdinalIgnoreCase));
        return name is null ? Invalid<CategoryType>(key, "Expense or Gain") : Enum.Parse<CategoryType>(name);
    }

    private TValue? Parsed<TValue>(string key, Func<string?, Result<TValue>> create)
        where TValue : class
    {
        var text = Text(key);
        if (text is null)
        {
            return null;
        }

        var result = create(text);
        if (result.IsFailure)
        {
            validation.AddResult(result.Error);
            return null;
        }

        return result.Value;
    }

    private JsonElement? Value(string key) =>
        data.TryGetValue(key, out var json) && json.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined) ? json : null;

    private TValue? Invalid<TValue>(string key, string expected)
        where TValue : struct
    {
        validation.AddResult(SyncDataErrors.FieldInvalid(key, expected));
        return null;
    }

    private TValue? InvalidReference<TValue>(string key, string expected)
        where TValue : class
    {
        validation.AddResult(SyncDataErrors.FieldInvalid(key, expected));
        return null;
    }
}
