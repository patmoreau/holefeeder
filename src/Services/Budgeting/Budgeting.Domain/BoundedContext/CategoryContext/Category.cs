using System;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext
{
    public record Category : IAggregateRoot
    {
        private readonly Guid _id;
        private readonly string _name = string.Empty;
        private readonly Guid _userId;

        public Guid Id
        {
            get=>_id;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(Id)} is required");
                }

                _id = value;
            }
        }
        
        public CategoryType Type { get; init; }
        
        public string Name
        {
            get=>_name;
            init
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 255)
                {
                    throw new HolefeederDomainException($"{nameof(Name)} must be from 1 to 255 characters");
                }

                _name = value;
            }
        }

        public string Color { get; init; }
        
        public bool Favorite { get; init; }
        
        public bool System { get; init; }
        
        public decimal BudgetAmount { get; init; }

        public Guid UserId
        {
            get => _userId;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(UserId)} is required");
                }

                _userId = value;
            }
        }
        
        public static Category Create(CategoryType type, string name, decimal budgetAmount,
            string description, Guid userId)
            => new()
            {
                Id = Guid.NewGuid(),
                Type = type,
                Name = name,
                Favorite = false,
                BudgetAmount = budgetAmount,
                UserId = userId,
            };
    }
}
