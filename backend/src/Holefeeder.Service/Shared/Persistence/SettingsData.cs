using System.Globalization;
using System.Text.Json;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Shared.Persistence;

internal static class SettingsData
{
    public static Result<PeriodSettings> ToPeriodSettings(string data, UnexpectedError invalid)
    {
        try
        {
            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;

            if (Property(root, "effectiveDate") is not { ValueKind: JsonValueKind.String } effectiveDateValue ||
                !DateTime.TryParse(effectiveDateValue.GetString(), CultureInfo.InvariantCulture, out var effectiveDate))
            {
                return invalid;
            }

            Result<IntervalType> intervalType = Property(root, "intervalType") is { ValueKind: JsonValueKind.String } intervalTypeValue
                ? IntervalType.Create(intervalTypeValue.GetString())
                : IntervalType.Monthly;
            if (intervalType.IsFailure)
            {
                return invalid;
            }

            if (Property(root, "frequency") is not { ValueKind: JsonValueKind.Number } frequencyValue ||
                !frequencyValue.TryGetInt32(out var frequency))
            {
                return invalid;
            }

            var settings = PeriodSettings.Create(DateOnly.FromDateTime(effectiveDate), intervalType.Value, frequency);
            return settings.IsFailure ? invalid : settings;
        }
        catch (JsonException)
        {
            return invalid;
        }
    }

    public static string FromPeriodSettings(PeriodSettings settings) =>
        JsonSerializer.Serialize(new
        {
            effectiveDate = settings.EffectiveDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            intervalType = JsonNamingPolicy.CamelCase.ConvertName(settings.IntervalType.Name),
            frequency = settings.Frequency
        });

    private static JsonElement? Property(JsonElement element, string name) =>
        element.ValueKind == JsonValueKind.Object
            ? element.EnumerateObject()
                .Where(property => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                .Select(property => (JsonElement?)property.Value)
                .FirstOrDefault()
            : null;
}
