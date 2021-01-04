using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    [DataContract]
    public class PayCashflowCommand : IRequest<bool>
    {
        [DataMember] private readonly List<string> _tags;

        [DataMember] public DateTime Date { get; }

        [DataMember] public decimal Amount { get; }

        [DataMember] public string Description { get; }

        [DataMember] public Guid AccountId { get; }

        [DataMember] public Guid CategoryId { get; }

        [DataMember] public Guid? CashflowId { get; }

        [DataMember] public DateTime? CashflowDate { get; }

        [DataMember] public Guid UserId { get; }

        [DataMember] public IEnumerable<string> Tags => _tags;

        public PayCashflowCommand()
        {
            _tags = new List<string>();
        }

        public PayCashflowCommand(List<string> tags, DateTime date, decimal amount, string description,
            Guid categoryId, Guid accountId, Guid? cashflowId, DateTime? cashflowDate, Guid userId) : this()
        {
            _tags = tags.ToList();
            Date = date;
            Amount = amount;
            Description = description;
            CategoryId = categoryId;
            AccountId = accountId;
            CashflowId = cashflowId;
            CashflowDate = cashflowDate;
            UserId = userId;
        }
    }
}
