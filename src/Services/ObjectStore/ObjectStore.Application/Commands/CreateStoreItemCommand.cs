using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Commands
{
    [DataContract]
    public class CreateStoreItemCommand : IRequest<CommandResult<Guid>>
    {
        [DataMember, Required] public string Code { get; set; }

        [DataMember, Required] public string Data { get; set; }

        public CreateStoreItemCommand() { }

        public CreateStoreItemCommand(string code, string data)
        {
            Code = code;
            Data = data;
        }
    }
}
