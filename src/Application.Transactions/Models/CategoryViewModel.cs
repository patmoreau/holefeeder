namespace DrifterApps.Holefeeder.Application.Transactions.Models
{
    public class CategoryViewModel
    {
        public string Name { get; }

        public string Color { get; }

        public decimal BudgetAmount { get; }

        public bool Favorite { get; }

        public CategoryViewModel(string name, string color, decimal budgetAmount, bool favorite)
        {
            Name = name;
            Color = color;
            BudgetAmount = budgetAmount;
            Favorite = favorite;
        }
    }
}
