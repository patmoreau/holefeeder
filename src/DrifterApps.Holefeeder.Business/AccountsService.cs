using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Enums;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.ResourcesAccess;

namespace DrifterApps.Holefeeder.Business
{
    public class AccountsService : BaseOwnedService<AccountEntity>, IAccountsService
    {
        private readonly IAccountsRepository _repository;
        private readonly ITransactionsService _transactionService;
        private readonly ICategoriesService _categoryService;

        public AccountsService(IAccountsRepository repository, ITransactionsService transactionService, ICategoriesService categoryService) : base(repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
            _transactionService = transactionService.ThrowIfNull(nameof(transactionService));
            _categoryService = categoryService.ThrowIfNull(nameof(categoryService));
        }

        public async Task<IEnumerable<AccountDetailEntity>> FindWithDetailsAsync(string userId, QueryParams queryParams)
        {
            var accounts = await _repository.FindAsync(userId, queryParams);
            var transactions = await _transactionService.FindAsync(userId, QueryParams.Empty);
            var categories = await _categoryService.FindAsync(userId, QueryParams.Empty);

            return
                (from a in accounts
                join t in transactions on a.Id equals t.Account
                join c in categories on t.Category equals c.Id
                select new
                {
                    Id = a.Id,
                    Type = a.Type,
                    Name = a.Name,
                    OpenBalance = a.OpenBalance,
                    Description = a.Description,
                    Favorite = a.Favorite,
                    Inactive = a.Inactive,
                    Date = t.Date,
                    Amount = t.Amount * (c.Type == CategoryType.Gain ? 1 : -1) * ((a.Type == AccountType.Checking || a.Type == AccountType.Investment || a.Type == AccountType.Savings) ? 1 : -1),
                })
                .GroupBy(t => t.Id)
                .Select(t => new AccountDetailEntity
                (
                    t.Key,
                    t.First().Type,
                    t.First().Name,
                    t.Count(),
                    t.First().OpenBalance + t.Sum(x => x.Amount),
                    t.Max(x => x.Date),
                    t.First().Description,
                    t.First().Favorite,
                    t.First().Inactive
                ));

        }
    }
}