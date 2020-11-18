using System;

namespace DrifterApps.Holefeeder.Application.Transactions.Models
{
    public class AccountInfoViewModel
    {
        public Guid Id { get; }

        public string Name { get; }

        public string MongoId { get; }

        public AccountInfoViewModel(Guid id, string name, string mongoId)
        {
            Id = id;
            Name = name;
            MongoId = mongoId;
        }
    }
}
