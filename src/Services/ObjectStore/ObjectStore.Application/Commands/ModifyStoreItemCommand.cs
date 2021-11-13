using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    public class ModifyStoreItemCommand : IRequest<bool>
    {
        [DataMember, Required] public Guid Id { get; init; }

        [DataMember, Required] public string Data { get; init; } = string.Empty;
    }
}
