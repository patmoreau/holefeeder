using System;

using DrifterApps.Holefeeder.Budgeting.Application.Imports.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Queries
{
    public record ImportDataStatusQuery(Guid RequestId) : IRequest<CommandResult<ImportDataStatusViewModel>>;
}
