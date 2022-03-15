using System.Text.Json.Serialization;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QueryStatus
{
    BadRequest,
    Error,
    NotFound,
    Ok
}
