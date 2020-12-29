using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    public class ModifyStoreItemCommand : IRequest<CommandResult<Unit>>
    {
        [DataMember, Required] public Guid Id { get; set; }

        [DataMember, Required] public string Data { get; set; }

        public ModifyStoreItemCommand() { }

        public ModifyStoreItemCommand(Guid id, string data)
        {
            Id = id;
            Data = data;
        }
    }
}
