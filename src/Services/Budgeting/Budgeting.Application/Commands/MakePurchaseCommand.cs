using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    [DataContract]
    public class MakePurchaseCommand : IRequest<bool>
    {
        [DataMember] private readonly List<string> _tags;

        [DataMember] public DateTime Date { get; }

        [DataMember] public decimal Amount { get; }

        [DataMember] public string Description { get; }

        [DataMember] public Guid AccountId { get; }

        [DataMember] public Guid CategoryId { get; }

        [DataMember] public Guid UserId { get; }

        [DataMember] public IEnumerable<string> Tags => _tags;

        public MakePurchaseCommand()
        {
            _tags = new List<string>();
        }

        public MakePurchaseCommand(List<string> tags, DateTime date, decimal amount, string description,
            Guid categoryId, Guid accountId, Guid userId) : this()
        {
            _tags = tags.ToList();
            Date = date;
            Amount = amount;
            Description = description;
            CategoryId = categoryId;
            AccountId = accountId;
            UserId = userId;
        }
    }
}
