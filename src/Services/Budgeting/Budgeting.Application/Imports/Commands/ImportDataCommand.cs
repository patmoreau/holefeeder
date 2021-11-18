using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Commands;

public record ImportDataCommand
    ([Required] bool UpdateExisting, [Required] JsonDocument Data) : IRequest<Guid>;
