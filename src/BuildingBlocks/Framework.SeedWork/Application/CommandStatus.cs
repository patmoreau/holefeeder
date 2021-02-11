using System.Text.Json.Serialization;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CommandStatus
    {
        Ok,
        Created,
        NotFound,
        BadRequest,
        Error,
        Conflict
    }
}
