using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Exports.Commands
{
    public record ImportDataCommand : IRequest<CommandResult<int>>
    {
        [Required]
        public bool UpdateExisting { get; init; }

        [Required]
        public JsonDocument Data { get; init; }
    }
}
