using System.Text.Json.Serialization;

namespace Holefeeder.Application.SeedWork;

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
