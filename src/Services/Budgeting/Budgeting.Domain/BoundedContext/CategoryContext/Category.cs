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
            get => _id;
            private init
            {
                if (value.Equals(default))
                {
                    throw HolefeederDomainException.Create<Category>($"{nameof(Id)} is required");
                }

                _id = value;
            }
        }

        public CategoryType Type { get; init; }

        public string Name
        {
            get => _name;
            init
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 255)
                {
                    throw HolefeederDomainException.Create<Category>($"{nameof(Name)} must be from 1 to 255 characters");
                }

                _name = value;
            }
        }

        public string Color { get; init; } = string.Empty;

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
                    throw HolefeederDomainException.Create<Category>($"{nameof(UserId)} is required");
                }

                _userId = value;
            }
        }

        public Category(Guid id, CategoryType type, string name, Guid userId)
        {
            Id = id;
            Type = type;
            Name = name;
            UserId = userId;
        }

        public static Category Create(CategoryType type, string name, decimal budgetAmount,
            string description, Guid userId)
            => new(Guid.NewGuid(), type, name, userId) { BudgetAmount = budgetAmount };
    }
}
