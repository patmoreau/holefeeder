using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class CategoryEntity : BaseEntity, IIdentityEntity<CategoryEntity>, IOwnedEntity<CategoryEntity>
    {
        public string Name { get; }
        public CategoryType Type { get; }
        public string Color { get; }
        public decimal BudgetAmount { get; }
        public bool Favorite { get; }
        public bool System { get; }
        public string UserId { get; }

        public CategoryEntity(string id, string name, CategoryType type, string color, decimal budgetAmount, bool favorite, bool system, string userId) : base(id)
        {
            Name = name;
            Type = type;
            Color = color;
            BudgetAmount = budgetAmount;
            Favorite = favorite;
            System = system;
            UserId = userId;
        }

        public CategoryEntity With(string id = null, string name = null, CategoryType? type = null, string color = null, decimal? budgetAmount = null, bool? favorite = null, bool? system = null, string userId = null) =>
            new CategoryEntity(
                id ?? Id,
                name ?? Name,
                type ?? Type,
                color ?? Color,
                budgetAmount ?? BudgetAmount,
                favorite ?? Favorite,
                system ?? System,
                userId ?? UserId
                );

        public CategoryEntity WithId(string id) => With(id);

        public CategoryEntity WithUser(string userId) => With(userId: userId);
    }
}
