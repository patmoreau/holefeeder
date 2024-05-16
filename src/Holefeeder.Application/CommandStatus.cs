using System.Text.Json.Serialization;

namespace Holefeeder.Application;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommandStatus
{
    Initializing,
    Accepted,
    BadRequest,
    Completed,
    Conflict,
    Created,
    Error,
    InProgress,
    NotFound,
    Ok
}
