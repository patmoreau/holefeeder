using System.Text.Json.Serialization;

namespace Holefeeder.Ui.Shared.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoryType
{
    Expense,
    Gain
}
