using System.Globalization;
using System.Text.Json;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;

namespace Holefeeder.Service.Dashboard.Persistence;

internal static class SettingsData
{
    public static Result<PeriodSettings> ToPeriodSettings(string data)
    {
        try
        {
            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;

            if (Property(root, "effectiveDate") is not { ValueKind: JsonValueKind.String } effectiveDateValue ||
                !DateTime.TryParse(effectiveDateValue.GetString(), CultureInfo.InvariantCulture, out var effectiveDate))
            {
                return DashboardErrors.SettingsInvalid;
            }

            Result<IntervalType> intervalType = Property(root, "intervalType") is { ValueKind: JsonValueKind.String } intervalTypeValue
                ? IntervalType.Create(intervalTypeValue.GetString())
                : IntervalType.Monthly;
            if (intervalType.IsFailure)
            {
                return DashboardErrors.SettingsInvalid;
            }

            if (Property(root, "frequency") is not { ValueKind: JsonValueKind.Number } frequencyValue ||
                !frequencyValue.TryGetInt32(out var frequency))
            {
                return DashboardErrors.SettingsInvalid;
            }

            var settings = PeriodSettings.Create(DateOnly.FromDateTime(effectiveDate), intervalType.Value, frequency);
            return settings.IsFailure ? DashboardErrors.SettingsInvalid : settings;
        }
        catch (JsonException)
        {
            return DashboardErrors.SettingsInvalid;
        }
    }

    private static JsonElement? Property(JsonElement element, string name) =>
        element.ValueKind == JsonValueKind.Object
            ? element.EnumerateObject()
                .Where(property => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                .Select(property => (JsonElement?)property.Value)
                .FirstOrDefault()
            : null;
}
