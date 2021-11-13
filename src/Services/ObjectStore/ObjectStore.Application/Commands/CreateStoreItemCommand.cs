using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    [DataContract]
    public class CreateStoreItemCommand : IRequest<Guid>
    {
        [DataMember, Required] public string Code { get; init; } = string.Empty;

        [DataMember, Required] public string Data { get; init; } = string.Empty;
    }
}
