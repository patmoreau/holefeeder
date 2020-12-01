using System;
using System.Runtime.Serialization;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    [DataContract]
    public class TransferMoneyBetweenAccountsCommand : IRequest<CommandResult<Guid>>
    {
        [DataMember] public DateTime Date { get; }

        [DataMember] public decimal Amount { get; }

        [DataMember] public string Description { get; }

        [DataMember] public Guid DebitAccountId { get; }

        [DataMember] public Guid CreditAccountId { get; }

        [DataMember] public Guid UserId { get; }

        public TransferMoneyBetweenAccountsCommand(DateTime date, decimal amount, string description,
            Guid debitAccountId, Guid creditAccountId, Guid userId)
        {
            Date = date;
            Amount = amount;
            Description = description;
            DebitAccountId = debitAccountId;
            CreditAccountId = creditAccountId;
            UserId = userId;
        }
    }
}
