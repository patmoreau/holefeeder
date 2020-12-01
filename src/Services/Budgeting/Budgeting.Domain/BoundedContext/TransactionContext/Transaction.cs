using System;
using System.Collections.Generic;
using System.Linq;
using DrifterApps.Holefeeder.Budgeting.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext
{
    public class Transaction : Entity, IAggregateRoot
    {
        public DateTime Date { get; }
        
        public decimal Amount { get; }
        
        public string Description { get; }
        
        public Guid AccountId { get; }
        
        public Guid CategoryId { get; }
        
        public Guid? CashflowId { get; }
        
        public DateTime? CashflowDate { get; }

        private readonly List<string> _tags;
        public IReadOnlyCollection<string> Tags => _tags;
        
        public Guid UserId { get; }

        public Transaction()
        {
            _tags = new List<string>();
        }

        public static Transaction Create(DateTime date, decimal amount, string description, Guid categoryId, Guid accountId, Guid? cashflowId, DateTime? cashflowDate, Guid userId)
        => new Transaction(Guid.NewGuid(), date, amount, description, categoryId, accountId, cashflowId, cashflowDate, userId);
        
        public Transaction(Guid id, DateTime date, decimal amount, string description, Guid categoryId, Guid accountId, Guid? cashflowId, DateTime? cashflowDate, Guid userId) : this()
        {
            Id = id;
            Date = date;
            Amount = amount;
            Description = description;
            CategoryId = categoryId;
            AccountId = accountId;
            CashflowId = cashflowId;
            CashflowDate = cashflowDate;
            UserId = userId;
        }
        
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return;
            }
            
            var existingTag = _tags.SingleOrDefault(o => string.Equals(o, tag, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(existingTag))
            {
                _tags.Add(tag.ToLowerInvariant());
            }
        }
    }
}
